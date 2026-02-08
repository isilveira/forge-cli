using Forge.CLI.Models;
using Forge.CLI.Persistence;
using static Forge.CLI.Shared.Helpers.ForgeHelper;
using Spectre.Console.Cli;
using Forge.CLI.Shared.Helpers;

namespace Forge.CLI.Commands.Add
{
	public sealed class AddPropertySettings : CommandSettings
	{
		[CommandArgument(0, "<property>")]
		public string Property { get; set; } = string.Empty;

		[CommandArgument(1, "<to>")]
		public string To { get; set; } = "to"; // literal obrigatório

		[CommandArgument(2, "<entity>")]
		public string Entity { get; set; } = string.Empty;

		[CommandArgument(3, "<on>")]
		public string On { get; set; } = "on"; // literal obrigatório

		[CommandArgument(4, "<context>")]
		public string Context { get; set; } = string.Empty;

		[CommandOption("-t|--type")]
		public string Type { get; set; } = "string";

		[CommandOption("--required")]
		public bool Required { get; set; }

		[CommandOption("--length")]
		public int? Length { get; set; } = 128;

		[CommandOption("--has-max-length")]
		public bool HasMaxLength { get; set; }

		[CommandOption("--precision")]
		public int? Precision { get; set; } = 18;

		[CommandOption("--scale")]
		public int? Scale { get; set; } = 2;
	}
	public sealed class AddPropertyCommand
	: AsyncCommand<AddPropertySettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			AddPropertySettings settings,
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

			if (entity.Properties.ContainsKey(settings.Property))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Property '{settings.Property}' already exists in entity '{settings.Entity}'.", "red");
				return -1;
			}

			entity.Properties[settings.Property] = new ForgeProperty
			{
				Type = settings.Type,
				Required = settings.Required,
				Length = settings.Length,
				HasMaxLength = settings.HasMaxLength,
				Precision = settings.Precision,
				Scale = settings.Scale
			};

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Property '{settings.Property}' added to entity '{settings.Entity}' in context '{settings.Context}'.");

			return 0;
		}
	}
}