using Forge.CLI.Core.Artifacts;
using Forge.CLI.Shared.Helpers;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Init
{
	public sealed class InitArtifactsSettings : CommandSettings
	{
		[CommandOption("--what-if")]
		public bool WhatIf { get; set; }

		[CommandOption("--force")]
		public bool Force { get; set; }

		[CommandOption("--yes")]
		public bool Yes { get; set; }
	}

	public sealed class InitArtifactsCommand : AsyncCommand<InitArtifactsSettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			InitArtifactsSettings settings,
			CancellationToken cancellationToken)
		{
			var projectRoot = Directory.GetCurrentDirectory();
			var exporter = new EmbeddedArtifactExporter();
			var items = exporter.Discover(projectRoot);

			if (items.Count == 0)
			{
				AnsiConsoleHelper.SafeMarkupLine(
					"Nenhum artefato embutido encontrado.",
					"yellow");
				return 1;
			}

			var conflicts = items.Where(i => i.Exists).ToList();
			if (conflicts.Count > 0 && settings.Force && !settings.Yes && !settings.WhatIf)
			{
				if (!AnsiConsoleHelper.SafeConfirm(
					$"Sobrescrever {conflicts.Count} artefato(s) existentes em .forge/Artifacts?"))
				{
					AnsiConsoleHelper.SafeMarkupLine("Operação cancelada.", "yellow");
					return 1;
				}
			}

			var result = await exporter.ExportAsync(
				projectRoot,
				force: settings.Force,
				whatIf: settings.WhatIf,
				cancellationToken);

			if (settings.WhatIf)
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"[what-if] {result.WouldWrite.Count} artefato(s) seriam copiados para .forge/Artifacts:",
					"blue");

				foreach (var path in result.WouldWrite)
					AnsiConsoleHelper.SafeMarkupLine($"  {path}", "blue");

				if (result.Skipped.Count > 0)
				{
					AnsiConsoleHelper.SafeMarkupLine(
						$"[what-if] {result.Skipped.Count} seriam ignorados (já existem; use --force).",
						"yellow");
				}

				return 0;
			}

			foreach (var error in result.Errors)
				AnsiConsoleHelper.SafeMarkupLine(error, "red");

			AnsiConsoleHelper.SafeMarkupLine(
				$"Artefatos copiados: {result.Written.Count} em .forge/Artifacts");

			if (result.Skipped.Count > 0)
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Ignorados (já existem): {result.Skipped.Count}. Use --force para sobrescrever.",
					"yellow");
			}

			return result.Errors.Count > 0 ? 1 : 0;
		}
	}
}
