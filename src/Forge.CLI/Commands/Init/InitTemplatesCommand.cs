using Forge.CLI.Core.Templates;
using Forge.CLI.Shared.Helpers;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Init
{
	public sealed class InitTemplatesSettings : CommandSettings
	{
		[CommandOption("--what-if")]
		public bool WhatIf { get; set; }

		[CommandOption("--force")]
		public bool Force { get; set; }

		[CommandOption("--yes")]
		public bool Yes { get; set; }
	}

	public sealed class InitTemplatesCommand : AsyncCommand<InitTemplatesSettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			InitTemplatesSettings settings,
			CancellationToken cancellationToken)
		{
			var projectRoot = Directory.GetCurrentDirectory();
			var exporter = new EmbeddedTemplateExporter();
			var items = exporter.Discover(projectRoot);

			if (items.Count == 0)
			{
				AnsiConsoleHelper.SafeMarkupLine(
					"Nenhum template embutido encontrado.",
					"yellow");
				return 1;
			}

			var conflicts = items.Where(i => i.Exists).ToList();
			if (conflicts.Count > 0 && settings.Force && !settings.Yes && !settings.WhatIf)
			{
				if (!AnsiConsoleHelper.SafeConfirm(
					$"Sobrescrever {conflicts.Count} template(s) existentes em .forge/Templates?"))
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
					$"[what-if] {result.WouldWrite.Count} template(s) seriam copiados para .forge/Templates:",
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
				$"Templates copiados: {result.Written.Count} em .forge/Templates");

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
