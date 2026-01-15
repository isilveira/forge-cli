using BAYSOFT.Abstractions.Crosscutting.Extensions;
using BAYSOFT.Abstractions.Crosscutting.Pluralization;
using BAYSOFT.Abstractions.Crosscutting.Pluralization.English;
using Forge.CLI.Models;
using Forge.CLI.Persistence;
using Forge.CLI.Shared.Helpers;
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

		[CommandOption("-n|--name")]
		public string? Name { get; set; } = default;

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

			if (!forgeContext.Entities.TryGetValue(settings.TargetEntity, out var targetEntity))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Target entity '{settings.TargetEntity}' not found in context '{settings.Context}'.", "red");
				return -1;
			}

			var sourceRelationName = settings.Name ?? settings.TargetEntity;

			if (sourceEntity.Relations.ContainsKey(sourceRelationName))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Relation '{sourceRelationName}' to '{settings.TargetEntity}' already exists in '{settings.SourceEntity}'.", "red");
				return -1;
			}

			sourceEntity.Relations[sourceRelationName] = new ForgeRelation
			{
				Type = "many-to-one",
				Target = settings.TargetEntity,
				Required = settings.Required
			};

			var sourceEntityCollectionName = Pluralizer.GetInstance()
				.AddEnglishPluralizer()
				.Pluralize(settings.SourceEntity, "en-US")
				.ToPascalCase();
			var targetRelationName = !string.IsNullOrWhiteSpace(settings.Name) && !settings.Name.ToLower().Equals(settings.TargetEntity.ToLower())
				? $"{settings.Name}{sourceEntityCollectionName}"
				: $"{sourceEntityCollectionName}";

			if (targetEntity.Relations.ContainsKey(targetRelationName))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Relation '{targetRelationName}' to '{settings.SourceEntity}' already exists in '{settings.TargetEntity}'.", "red");
				return -1;
			}

			targetEntity.Relations[$"{targetRelationName}"] = new ForgeRelation
			{
				Type = "one-to-many",
				Target = settings.SourceEntity,
				Required = false
			};

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Relation '{sourceRelationName}' from '{settings.SourceEntity}' to '{settings.TargetEntity}' added in context '{settings.Context}'.");

			return 0;
		}
	}
}