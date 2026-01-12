using Forge.CLI.Shared.Helpers;
using Forge.CLI.Tools;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Remove
{
	// Settings
	public sealed class RemoveEntitySettings : CommandSettings
	{
		[CommandArgument(0, "<entity>")]
		public string Entity { get; set; } = string.Empty;

		[CommandArgument(1, "<from>")]
		public string From { get; set; } = "from";

		[CommandArgument(2, "<context>")]
		public string Context { get; set; } = string.Empty;

		[CommandOption("-f|--force")]
		public bool Force { get; set; } = false;
	}
	public sealed class RemoveEntityCommand : AsyncCommand<RemoveEntitySettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			RemoveEntitySettings settings,
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

			if (!project.Contexts.TryGetValue(settings.Context, out var forgeContext))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Context '{settings.Context}' not found.", "red");
				return -1;
			}

			if (!forgeContext.Entities.ContainsKey(settings.Entity))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Entity '{settings.Entity}' not found in context '{settings.Context}'.", "red");
				return -1;
			}

			if (!settings.Force && !AnsiConsoleHelper.SafeConfirm($"Are you sure you want to remove the entity {settings.Entity} from context '{settings.Context}'?"))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					"Operation canceled.", "yellow");

				return 0;
			}

			forgeContext.Entities.Remove(settings.Entity);

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Entity '{settings.Entity}' removed from context '{settings.Context}'.");

			return 0;
		}
	}
}
