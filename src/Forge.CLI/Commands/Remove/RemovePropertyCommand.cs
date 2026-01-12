using Forge.CLI.Shared.Helpers;
using Forge.CLI.Tools;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Remove
{
	// Settings
	public sealed class RemovePropertySettings : CommandSettings
	{
		[CommandArgument(0, "<property>")]
		public string Property { get; set; } = string.Empty;

		[CommandArgument(1, "<from>")]
		public string From { get; set; } = "from";

		[CommandArgument(2, "<entity>")]
		public string Entity { get; set; } = string.Empty;

		[CommandArgument(3, "<on>")]
		public string On { get; set; } = "on";

		[CommandArgument(4, "<context>")]
		public string Context { get; set; } = string.Empty;

		[CommandOption("-f|--force")]
		public bool Force { get; set; } = false;
	}
	public sealed class RemovePropertyCommand : AsyncCommand<RemovePropertySettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			RemovePropertySettings settings,
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

			if (!forgeContext.Entities.TryGetValue(settings.Entity, out var entity))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Entity '{settings.Entity}' not found in context '{settings.Context}'.", "red");
				return -1;
			}

			if (!entity.Properties.ContainsKey(settings.Property))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Property '{settings.Property}' not found in entity '{settings.Entity}'.", "red");
				return -1;
			}

			if (!settings.Force && !AnsiConsoleHelper.SafeConfirm($"Are you sure you want to remove the property {settings.Property} from entity {settings.Entity} on context '{settings.Context}'?"))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					"Operation canceled.", "yellow");

				return 0;
			}

			entity.Properties.Remove(settings.Property);

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine($"[green]Property '{settings.Property}' removed from entity '{settings.Entity}'.[/]");

			return 0;
		}
	}

}
