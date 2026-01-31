using Forge.CLI.Core.CodeScanning.Aggregation;
using Forge.CLI.Core.CodeScanning.Conversion;
using Forge.CLI.Core.CodeScanning.Markers;
using Forge.CLI.Core.CodeScanning.Merging;
using Forge.CLI.Core.CodeScanning.Parsing;
using Forge.CLI.Core.CodeScanning.Scanning;
using Forge.CLI.Models;
using Forge.CLI.Persistence;
using Forge.CLI.Shared.Helpers;

namespace Forge.CLI.Core.CodeScanning.Pipeline
{
	/// <summary>
	/// Pipeline completo de scanning de marcações no código-fonte.
	/// 
	/// Fluxo de execução:
	/// 1. [LOAD]      → Carrega projeto existente (se houver)
	/// 2. [SCAN]      → Escaneia arquivos em busca de marcações <forge:...>
	/// 3. [PARSE]     → Converte texto bruto em objetos tipados (markers)
	/// 4. [AGGREGATE] → Agrupa markers por tipo (entities, properties, relations)
	/// 5. [CONVERT]   → Transforma ScannedProjectModel em ForgeProject
	/// 6. [MERGE]     → Combina projeto escaneado com existente (sem sobrescrever)
	/// 7. [SAVE]      → Persiste em .forge/project.json via ProjectSaver
	/// 
	/// Responsabilidade: orquestrar o fluxo, delegando cada etapa para serviços especializados.
	/// </summary>
	public sealed class ForgeScanPipeline
	{
		private readonly FileScanner _scanner = new();
		private readonly MarkerParser _parser = new();
		private readonly MarkerAggregator _aggregator = new();
		private readonly ScannedModelConverter _converter = new();
		private readonly ProjectMerger _merger = new();
		private readonly ProjectLoader _loader = new();
		private readonly ProjectSaver _saver = new();

		/// <summary>
		/// Executa o pipeline completo de scanning.
		/// </summary>
		/// <param name="options">Opções de scan (diretório, extensões, etc.)</param>
		/// <param name="mergeOptions">Opções de merge (overwrite, create, etc.)</param>
		/// <param name="dryRun">Se true, não salva alterações no disco</param>
		/// <returns>Resultado do pipeline com estatísticas</returns>
		public async Task<ScanPipelineResult> RunAsync(
			ScanOptions? options = null,
			MergeOptions? mergeOptions = null,
			bool dryRun = false)
		{
			options ??= new ScanOptions();
			mergeOptions ??= MergeOptions.AddOnly;

			var result = new ScanPipelineResult();

			// ══════════════════════════════════════════════════════════════
			// 1. [LOAD] Carregar projeto existente
			// ══════════════════════════════════════════════════════════════
			AnsiConsoleHelper.SafeMarkupLine("[LOAD] loading existing project...", "blue");

			var existingProject = _loader.TryLoad();
			var isNewProject = existingProject is null;

			if (isNewProject)
			{
				AnsiConsoleHelper.SafeMarkupLine("[LOAD] no existing project found, will create new", "yellow");
				existingProject = new ForgeProject
				{
					Name = Path.GetFileName(Directory.GetCurrentDirectory()),
					Contexts = new Dictionary<string, ForgeContext>()
				};
			}
			else
			{
				AnsiConsoleHelper.SafeMarkupLine($"[LOAD] loaded project: {existingProject!.Name}", "green");
			}

			// ══════════════════════════════════════════════════════════════
			// 2. [SCAN] Escanear arquivos
			// ══════════════════════════════════════════════════════════════
			AnsiConsoleHelper.SafeMarkupLine("[SCAN] scanning files...", "blue");

			var files = _scanner.Scan(options).ToList();
			result.FilesScanned = files.Count;

			AnsiConsoleHelper.SafeMarkupLine($"[SCAN] found {files.Count} file(s)", "gray");

			// ══════════════════════════════════════════════════════════════
			// 3. [EXTRACT] Extrair marcações brutas dos arquivos
			// ══════════════════════════════════════════════════════════════
			var rawMarkers = new List<RawForgeMarker>();

			foreach (var file in files)
			{
				var lines = await File.ReadAllLinesAsync(file);

				for (int i = 0; i < lines.Length; i++)
				{
					if (lines[i].Contains("<forge:"))
					{
						rawMarkers.Add(new RawForgeMarker(file, i + 1, lines[i]));
					}
				}
			}

			result.MarkersFound = rawMarkers.Count;
			AnsiConsoleHelper.SafeMarkupLine($"[EXTRACT] found {rawMarkers.Count} marker(s)", "gray");

			if (rawMarkers.Count == 0)
			{
				AnsiConsoleHelper.SafeMarkupLine("[SKIP] no markers found, nothing to do", "yellow");
				result.Skipped = true;
				return result;
			}

			// ══════════════════════════════════════════════════════════════
			// 4. [PARSE] Converter marcações brutas em objetos tipados
			// ══════════════════════════════════════════════════════════════
			AnsiConsoleHelper.SafeMarkupLine("[PARSE] parsing markers...", "blue");

			var parsed = new List<object>();
			var parseErrors = new List<string>();

			foreach (var raw in rawMarkers)
			{
				try
				{
					parsed.Add(_parser.Parse(raw));
				}
				catch (ForgeMarkerParseException ex)
				{
					parseErrors.Add(ex.Message);
					result.ParseErrors.Add(ex.Message);
				}
			}

			if (parseErrors.Count > 0)
			{
				AnsiConsoleHelper.SafeMarkupLine($"[PARSE] {parseErrors.Count} error(s):", "red");
				foreach (var err in parseErrors)
					AnsiConsoleHelper.SafeMarkupLine($"  - {err}", "red");
			}

			AnsiConsoleHelper.SafeMarkupLine($"[PARSE] parsed {parsed.Count} marker(s)", "gray");

			// ══════════════════════════════════════════════════════════════
			// 5. [AGGREGATE] Agrupar markers por tipo
			// ══════════════════════════════════════════════════════════════
			AnsiConsoleHelper.SafeMarkupLine("[AGGREGATE] building scanned model...", "blue");

			var scannedModel = _aggregator.Aggregate(parsed);

			AnsiConsoleHelper.SafeMarkupLine(
				$"[AGGREGATE] {scannedModel.Entities.Count} entity(ies), " +
				$"{scannedModel.Properties.Count} property(ies), " +
				$"{scannedModel.Relationships.Count} relationship(s)", "gray");

			// ══════════════════════════════════════════════════════════════
			// 6. [CONVERT] Transformar em ForgeProject
			// ══════════════════════════════════════════════════════════════
			AnsiConsoleHelper.SafeMarkupLine("[CONVERT] converting to ForgeProject...", "blue");

			var scannedProject = _converter.Convert(
				scannedModel,
				existingProject.Name);

			// ══════════════════════════════════════════════════════════════
			// 7. [MERGE] Combinar com projeto existente
			// ══════════════════════════════════════════════════════════════
			AnsiConsoleHelper.SafeMarkupLine("[MERGE] merging with existing project...", "blue");

			var mergeResult = _merger.Merge(existingProject, scannedProject, mergeOptions);
			result.MergeResult = mergeResult;

			AnsiConsoleHelper.SafeMarkupLine($"[MERGE] {mergeResult.GetSummary()}", "cyan");

			if (mergeResult.Warnings.Count > 0)
			{
				foreach (var warning in mergeResult.Warnings)
					AnsiConsoleHelper.SafeMarkupLine($"[WARN] {warning}", "yellow");
			}

			// ══════════════════════════════════════════════════════════════
			// 8. [SAVE] Persistir no project.json
			// ══════════════════════════════════════════════════════════════
			if (dryRun)
			{
				AnsiConsoleHelper.SafeMarkupLine("[DRY-RUN] skipping save (no changes written to disk)", "yellow");
				result.ProjectSaved = false;
			}
			else if (mergeResult.HasChanges || isNewProject)
			{
				AnsiConsoleHelper.SafeMarkupLine("[SAVE] writing project.json...", "blue");
				await _saver.SaveAsync(existingProject);
				result.ProjectSaved = true;
				AnsiConsoleHelper.SafeMarkupLine("[DONE] project.json updated successfully", "green");
			}
			else
			{
				AnsiConsoleHelper.SafeMarkupLine("[SKIP] no changes to save", "gray");
			}

			return result;
		}
	}

	/// <summary>
	/// Resultado da execução do pipeline de scanning.
	/// </summary>
	public sealed class ScanPipelineResult
	{
		/// <summary>
		/// Número de arquivos escaneados.
		/// </summary>
		public int FilesScanned { get; set; }

		/// <summary>
		/// Número de marcações encontradas.
		/// </summary>
		public int MarkersFound { get; set; }

		/// <summary>
		/// Erros de parsing encontrados.
		/// </summary>
		public List<string> ParseErrors { get; } = [];

		/// <summary>
		/// Resultado do merge.
		/// </summary>
		public MergeResult? MergeResult { get; set; }

		/// <summary>
		/// Indica se o projeto foi salvo.
		/// </summary>
		public bool ProjectSaved { get; set; }

		/// <summary>
		/// Indica se a execução foi pulada (sem marcações).
		/// </summary>
		public bool Skipped { get; set; }

		/// <summary>
		/// Indica se a execução foi bem-sucedida.
		/// </summary>
		public bool Success => ParseErrors.Count == 0;
	}
}