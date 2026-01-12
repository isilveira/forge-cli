using Forge.CLI.Shared.Helpers;
using Forge.CLI.Tools;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Remove
{
    public sealed class RemoveContextSettings : CommandSettings
	{
		[CommandArgument(0, "<context>")]
		public string Context { get; set; } = string.Empty;

		[CommandOption("-f|--force")]
		public bool Force { get; set; } = false;
	}
    public sealed class RemoveContextCommand : AsyncCommand<RemoveContextSettings>
	{

		public override async Task<int> ExecuteAsync(
			CommandContext context,
			RemoveContextSettings settings,
			CancellationToken cancellationToken)
		{
			var loader = new ProjectLoader();
			var saver = new ProjectSaver();

			var project = loader.TryLoad();

			if (project is null)
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Forge not inicialized on this project!", "red");
				return -1;
			}

			if (!project.Contexts.ContainsKey(settings.Context))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Context '{settings.Context}' not found.", "red");
				return -1;
			}

			if (!settings.Force && !AnsiConsoleHelper.SafeConfirm($"Are you sure you want to remove the context '{settings.Context}'?"))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					"Operation canceled.", "yellow");
				return 0;
			}

			project.Contexts.Remove(settings.Context);

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine($"Context '{settings.Context}' removed.");

			return 0;
		}
	}
}
