using Forge.CLI.Shared.Helpers;
using Forge.CLI.Tools;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Update
{
	// Settings
	public sealed class UpdateContextSettings : CommandSettings
	{
		[CommandArgument(0, "<context>")]
		public string Context { get; set; } = string.Empty;

		[CommandOption("--name")]
		public string? NewName { get; set; } = null;

		[CommandOption("-d|--description <DESCRIPTION>")]
		public string? Description { get; set; } = null;
	}
	public sealed class UpdateContextCommand : AsyncCommand<UpdateContextSettings>
	{

		public override async Task<int> ExecuteAsync(CommandContext context, UpdateContextSettings settings, CancellationToken cancellationToken)
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

			if (settings.Description is not null)
			{
				forgeContext.Description = settings.Description;

				AnsiConsoleHelper.SafeMarkupLine(
					$"Context '{settings.Context}' decription set to '{settings.Description}'.");
			}

			if (settings.NewName is not null)
			{
				project.Contexts.Remove(settings.Context);
				project.Contexts[settings.NewName] = forgeContext;

				AnsiConsoleHelper.SafeMarkupLine(
					$"Context '{settings.Context}' renamed to '{settings.NewName}'.");
			}

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Context '{settings.Context}' updated'.");

			return 0;
		}
	}
}
