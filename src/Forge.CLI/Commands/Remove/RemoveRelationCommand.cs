using Forge.CLI.Shared.Helpers;
using Forge.CLI.Tools;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Remove
{

	// Settings
	public sealed class RemoveRelationSettings : CommandSettings
	{
		[CommandArgument(0, "<targetEntity>")]
		public string TargetEntity { get; set; } = string.Empty;

		[CommandArgument(1, "<from>")]
		public string From { get; set; } = "from";

		[CommandArgument(2, "<sourceEntity>")]
		public string SourceEntity { get; set; } = string.Empty;

		[CommandArgument(3, "<on>")]
		public string On { get; set; } = "on";

		[CommandArgument(4, "<context>")]
		public string Context { get; set; } = string.Empty;

		[CommandOption("-f|--force")]
		public bool Force { get; set; } = false;
	}
	public sealed class RemoveRelationCommand : AsyncCommand<RemoveRelationSettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			RemoveRelationSettings settings,
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

			if (!forgeContext.Entities.TryGetValue(settings.SourceEntity, out var sourceEntity))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Source entity '{settings.SourceEntity}' not found in context '{settings.Context}'.", "red");
				return -1;
			}

			if (!sourceEntity.Relations.ContainsKey(settings.TargetEntity))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Relation to '{settings.TargetEntity}' not found in entity '{settings.SourceEntity}'.","red");
				return -1;
			}

			if (!settings.Force && !AnsiConsoleHelper.SafeConfirm($"Are you sure you want to remove the relation between {settings.TargetEntity} and {settings.SourceEntity} on context '{settings.Context}'?"))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					"Operation canceled.", "yellow");

				return 0;
			}

			sourceEntity.Relations.Remove(settings.TargetEntity);
			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Relation from '{settings.SourceEntity}' to '{settings.TargetEntity}' removed.");

			return 0;
		}
	}
}