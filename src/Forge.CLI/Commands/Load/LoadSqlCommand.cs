using Forge.CLI.Core.CodeScanning.Merging;
using Forge.CLI.Core.SqlLoading;
using Forge.CLI.Models;
using Forge.CLI.Persistence;
using Forge.CLI.Shared.Helpers;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Load
{
	/// <summary>
	/// Configurações do comando load sql.
	/// </summary>
	public sealed class LoadSqlSettings : CommandSettings
	{
		[CommandArgument(0, "<file>")]
		[System.ComponentModel.Description("Path to the SQL script file (CREATE TABLE style, e.g. EF migrations)")]
		public string File { get; set; } = string.Empty;

		[CommandOption("-c|--context <CONTEXT>")]
		[System.ComponentModel.Description("Force a single Forge context name for all tables. If omitted, each SQL schema becomes a separate context (dbo, Sales, etc.).")]
		public string? Context { get; set; }

		[CommandOption("-n|--project-name <NAME>")]
		[System.ComponentModel.Description("Project name (default: current directory name or existing project name)")]
		public string? ProjectName { get; set; }

		[CommandOption("--merge")]
		[System.ComponentModel.Description("Merge with existing project instead of replacing")]
		public bool Merge { get; set; } = false;

		[CommandOption("--merge-add-only")]
		[System.ComponentModel.Description("Merge with existing project instead of replacing with Add Olny properties")]
		public bool MergeAddOnly { get; set; } = false;

		[CommandOption("--merge-overwrite-all")]
		[System.ComponentModel.Description("Merge with existing project instead of replacing with Overwrite All properties")]
		public bool MergeOverwriteAll { get; set; } = false;

		[CommandOption("--dry-run")]
		[System.ComponentModel.Description("Show what would be done without saving")]
		public bool DryRun { get; set; } = false;
	}

	/// <summary>
	/// Comando que carrega o modelo do projeto (ForgeProject) a partir de um script SQL
	/// de criação de tabelas (estilo das migrations do Entity Framework).
	/// </summary>
	public sealed class LoadSqlCommand : AsyncCommand<LoadSqlSettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			LoadSqlSettings settings,
			CancellationToken cancellationToken)
		{
			var loader = new ProjectLoader();
			var saver = new ProjectSaver();
			var parser = new SqlScriptParser();
			var converter = new SqlToForgeProjectConverter();

			// Validar arquivo
			var filePath = Path.IsPathRooted(settings.File)
				? settings.File
				: Path.Combine(Directory.GetCurrentDirectory(), settings.File);

			if (!File.Exists(filePath))
			{
				AnsiConsoleHelper.SafeMarkupLine($"File not found: {filePath}", "red");
				return 1;
			}

			AnsiConsoleHelper.SafeMarkupLine($"[LOAD] reading SQL file: {filePath}", "blue");

			string sqlContent;
			try
			{
				sqlContent = await File.ReadAllTextAsync(filePath, cancellationToken);
			}
			catch (Exception ex)
			{
				AnsiConsoleHelper.SafeMarkupLine($"Failed to read file: {ex.Message}", "red");
				return 1;
			}

			if (string.IsNullOrWhiteSpace(sqlContent))
			{
				AnsiConsoleHelper.SafeMarkupLine("SQL file is empty.", "yellow");
				return 1;
			}

			// Parse
			AnsiConsoleHelper.SafeMarkupLine("[PARSE] parsing SQL script...", "blue");

			ParsedSqlModel parsed;
			try
			{
				parsed = parser.Parse(sqlContent);
			}
			catch (Exception ex)
			{
				AnsiConsoleHelper.SafeMarkupLine($"Parse error: {ex.Message}", "red");
				return 1;
			}

			if (parsed.Tables.Count == 0)
			{
				AnsiConsoleHelper.SafeMarkupLine("No CREATE TABLE statements found in the script.", "yellow");
				return 0;
			}

			AnsiConsoleHelper.SafeMarkupLine($"[PARSE] found {parsed.Tables.Count} table(s)", "gray");

			// Determinar nome do projeto
			var existingProject = loader.TryLoad();
			var projectName = settings.ProjectName
				?? existingProject?.Name
				?? Path.GetFileName(Directory.GetCurrentDirectory()) ?? "Project";

			// Converter para ForgeProject
			AnsiConsoleHelper.SafeMarkupLine("[CONVERT] building ForgeProject...", "blue");

			var sqlProject = converter.Convert(
				parsed,
				projectName,
				settings.Context);

			AnsiConsoleHelper.SafeMarkupLine(
				$"[CONVERT] {sqlProject.Contexts.Values.Sum(c => c.Entities.Count)} entity(ies), " +
				$"{sqlProject.Contexts.Values.Sum(c => c.Entities.Values.Sum(e => e.Properties.Count))} property(ies), " +
				$"{sqlProject.Contexts.Values.Sum(c => c.Entities.Values.Sum(e => e.Relations.Count))} relation(s)",
				"gray");

			if (settings.DryRun)
			{
				AnsiConsoleHelper.SafeMarkupLine("[DRY-RUN] skipping save (no changes written to disk)", "yellow");
				return 0;
			}

			ForgeProject finalProject;

			if (settings.Merge && existingProject != null)
			{
				AnsiConsoleHelper.SafeMarkupLine("[MERGE] merging with existing project...", "blue");
				var merger = new ProjectMerger();
				var mergeOptions = settings.MergeOverwriteAll ? MergeOptions.OverwriteAll : MergeOptions.OverwriteAll; // adicionar novos ou sobrescrever
				merger.Merge(existingProject, sqlProject, mergeOptions);
				finalProject = existingProject;
				AnsiConsoleHelper.SafeMarkupLine("[MERGE] merge completed", "gray");
			}
			else
			{
				finalProject = sqlProject;
			}

			// Salvar
			AnsiConsoleHelper.SafeMarkupLine("[SAVE] writing project.json...", "blue");

			try
			{
				finalProject.Sharpen();
				await saver.SaveAsync(finalProject);
				AnsiConsoleHelper.SafeMarkupLine("Project loaded successfully from SQL script.", "green");
			}
			catch (Exception ex)
			{
				AnsiConsoleHelper.SafeMarkupLine($"Failed to save: {ex.Message}", "red");
				return 1;
			}

			return 0;
		}
	}
}
