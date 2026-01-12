using Forge.CLI.Models;
using Forge.CLI.Shared.Helpers;
using Forge.CLI.Tools;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Add
{
	public sealed class AddRelationSettings : CommandSettings
	{
		[CommandArgument(0, "<targetEntity>")]
		public string TargetEntity { get; set; } = string.Empty;

		[CommandArgument(1, "<to>")]
		public string To { get; set; } = "to"; // literal obrigatório

		[CommandArgument(2, "<sourceEntity>")]
		public string SourceEntity { get; set; } = string.Empty;

		[CommandArgument(3, "<on>")]
		public string On { get; set; } = "on"; // literal obrigatório

		[CommandArgument(4, "<context>")]
		public string Context { get; set; } = string.Empty;

		[CommandOption("-t|--type")]
		public string Type { get; set; } = "many-to-one";

		[CommandOption("--required")]
		public bool Required { get; set; }
	}
	public sealed class AddRelationCommand
	: AsyncCommand<AddRelationSettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			AddRelationSettings settings,
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

			if (!forgeContext.Entities.ContainsKey(settings.TargetEntity))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Target entity '{settings.TargetEntity}' not found in context '{settings.Context}'.", "red");
				return -1;
			}

			if (sourceEntity.Relations.ContainsKey(settings.TargetEntity))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Relation to '{settings.TargetEntity}' already exists in '{settings.SourceEntity}'.", "red");
				return -1;
			}

			sourceEntity.Relations[settings.TargetEntity] = new ForgeRelation
			{
				Target = settings.TargetEntity,
				Type = settings.Type,
				Required = settings.Required
			};

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Relation '{settings.Type}' from '{settings.SourceEntity}' to '{settings.TargetEntity}' added in context '{settings.Context}'.");

			return 0;
		}
	}
}