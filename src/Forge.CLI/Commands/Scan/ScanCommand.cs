using Forge.CLI.Core.CodeScanning.Merging;
using Forge.CLI.Core.CodeScanning.Pipeline;
using Forge.CLI.Core.CodeScanning.Scanning;
using Forge.CLI.Shared.Helpers;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Scan
{
	/// <summary>
	/// Configurações do comando scan.
	/// Permite parametrizar o comportamento do scan e merge.
	/// </summary>
	public sealed class ScanSettings : CommandSettings
	{
		[CommandOption("-p|--path <PATH>")]
		[System.ComponentModel.Description("Root path to scan (default: current directory)")]
		public string? Path { get; set; }

		[CommandOption("-e|--extensions <EXTENSIONS>")]
		[System.ComponentModel.Description("File extensions to scan, comma-separated (default: .cs)")]
		public string? Extensions { get; set; }

		[CommandOption("--overwrite-entities")]
		[System.ComponentModel.Description("Overwrite existing entities with scanned data")]
		public bool OverwriteEntities { get; set; } = false;

		[CommandOption("--overwrite-properties")]
		[System.ComponentModel.Description("Overwrite existing properties with scanned data")]
		public bool OverwriteProperties { get; set; } = false;

		[CommandOption("--overwrite-relations")]
		[System.ComponentModel.Description("Overwrite existing relations with scanned data")]
		public bool OverwriteRelations { get; set; } = false;

		[CommandOption("--overwrite-all")]
		[System.ComponentModel.Description("Overwrite all existing data (entities, properties, relations)")]
		public bool OverwriteAll { get; set; } = false;

		[CommandOption("--no-create-contexts")]
		[System.ComponentModel.Description("Do not create missing contexts")]
		public bool NoCreateContexts { get; set; } = false;

		[CommandOption("--no-create-entities")]
		[System.ComponentModel.Description("Do not create missing entities")]
		public bool NoCreateEntities { get; set; } = false;

		[CommandOption("--no-create-properties")]
		[System.ComponentModel.Description("Do not create missing properties")]
		public bool NoCreateProperties { get; set; } = false;

		[CommandOption("--no-create-relations")]
		[System.ComponentModel.Description("Do not create missing relations")]
		public bool NoCreateRelations { get; set; } = false;

		[CommandOption("--dry-run")]
		[System.ComponentModel.Description("Show what would be done without making changes")]
		public bool DryRun { get; set; } = false;
	}

	/// <summary>
	/// Comando que escaneia o código-fonte em busca de marcações Forge
	/// e atualiza o projeto (.forge/project.json) com os dados encontrados.
	/// </summary>
	public sealed class ScanCommand : AsyncCommand<ScanSettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			ScanSettings settings,
			CancellationToken cancellationToken)
		{
			// Construir ScanOptions a partir das configurações
			var scanOptions = BuildScanOptions(settings);

			// Construir MergeOptions a partir das configurações
			var mergeOptions = BuildMergeOptions(settings);

			// Exibir configurações se dry-run
			if (settings.DryRun)
			{
				AnsiConsoleHelper.SafeMarkupLine("[DRY-RUN] Simulating scan without saving changes", "yellow");
				AnsiConsoleHelper.SafeMarkupLine($"  Path: {scanOptions.RootPath}", "gray");
				AnsiConsoleHelper.SafeMarkupLine($"  Extensions: {string.Join(", ", scanOptions.AllowedExtensions)}", "gray");
				AnsiConsoleHelper.SafeMarkupLine($"  Overwrite: entities={mergeOptions.OverwriteEntities}, properties={mergeOptions.OverwriteProperties}, relations={mergeOptions.OverwriteRelations}", "gray");
			}

			// Executar pipeline
			var pipeline = new ForgeScanPipeline();

			try
			{
				var result = await pipeline.RunAsync(scanOptions, mergeOptions, settings.DryRun);

				// Exibir resultado
				if (result.Skipped)
				{
					AnsiConsoleHelper.SafeMarkupLine("No markers found in scanned files.", "yellow");
					return 0;
				}

				if (result.ParseErrors.Count > 0)
				{
					AnsiConsoleHelper.SafeMarkupLine($"Scan completed with {result.ParseErrors.Count} parse error(s).", "yellow");
					return 1;
				}

				if (result.ProjectSaved)
				{
					AnsiConsoleHelper.SafeMarkupLine("Project updated successfully.", "green");
				}
				else if (settings.DryRun)
				{
					AnsiConsoleHelper.SafeMarkupLine("[DRY-RUN] No changes were saved.", "yellow");
				}
				else
				{
					AnsiConsoleHelper.SafeMarkupLine("No changes to save.", "gray");
				}

				return 0;
			}
			catch (Exception ex)
			{
				AnsiConsoleHelper.SafeMarkupLine($"Scan failed: {ex.Message}", "red");
				return 1;
			}
		}

		/// <summary>
		/// Constrói as opções de scan a partir das configurações do comando.
		/// </summary>
		private static ScanOptions BuildScanOptions(ScanSettings settings)
		{
			var options = new ScanOptions();

			if (!string.IsNullOrWhiteSpace(settings.Path))
			{
				options = options with { RootPath = settings.Path };
			}

			if (!string.IsNullOrWhiteSpace(settings.Extensions))
			{
				var extensions = settings.Extensions
					.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
					.Select(e => e.StartsWith('.') ? e : $".{e}")
					.ToArray();

				options = options with { AllowedExtensions = extensions };
			}

			return options;
		}

		/// <summary>
		/// Constrói as opções de merge a partir das configurações do comando.
		/// </summary>
		private static MergeOptions BuildMergeOptions(ScanSettings settings)
		{
			// Se --overwrite-all, usar preset OverwriteAll
			if (settings.OverwriteAll)
			{
				return MergeOptions.OverwriteAll;
			}

			// Caso contrário, construir a partir das flags individuais
			return new MergeOptions
			{
				OverwriteEntities = settings.OverwriteEntities,
				OverwriteProperties = settings.OverwriteProperties,
				OverwriteRelations = settings.OverwriteRelations,
				CreateMissingContexts = !settings.NoCreateContexts,
				CreateMissingEntities = !settings.NoCreateEntities,
				CreateMissingProperties = !settings.NoCreateProperties,
				CreateMissingRelations = !settings.NoCreateRelations
			};
		}
	}
}
