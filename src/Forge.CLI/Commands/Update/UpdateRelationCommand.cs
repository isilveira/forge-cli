using Forge.CLI.Shared.Helpers;
using Forge.CLI.Tools;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Update
{
	// Settings
	public sealed class UpdateRelationSettings : CommandSettings
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

		[CommandOption("--type")]
		public string? Type { get; set; }

		[CommandOption("--required")]
		public bool? Required { get; set; }
	}
	public sealed class UpdateRelationCommand : AsyncCommand<UpdateRelationSettings>
	{
		public override async Task<int> ExecuteAsync(CommandContext context, UpdateRelationSettings settings, CancellationToken cancellationToken)
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
					$"Source entity '{settings.SourceEntity}' not found.", "red");
				return -1;
			}

			if (!sourceEntity.Relations.TryGetValue(settings.TargetEntity, out var relation))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Relation to '{settings.TargetEntity}' not found.", "red");
				return -1;
			}

			if (!string.IsNullOrWhiteSpace(settings.Type))
			{
				relation.Type = settings.Type;
				AnsiConsoleHelper.SafeMarkupLine(
					$"Relation '{settings.SourceEntity} → {settings.TargetEntity}' type updated to '{settings.Type}'.");
			}

			if (settings.Required.HasValue)
			{
				relation.Required = settings.Required.Value;
				AnsiConsoleHelper.SafeMarkupLine(
					$"Relation '{settings.SourceEntity} → {settings.TargetEntity}' required set to '{settings.Required.Value}'.");
			}

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Relation '{settings.SourceEntity} → {settings.TargetEntity}' updated.");

			return 0;
		}
	}

}
