using Forge.CLI.Models;
using Forge.CLI.Persistence;
using static Forge.CLI.Shared.Helpers.ForgeHelper;
using Spectre.Console.Cli;
using Forge.CLI.Shared.Helpers;

namespace Forge.CLI.Commands.Add
{
	public sealed class AddEntitySettings : CommandSettings
	{
		[CommandArgument(0, "<entity>")]
		public string Entity { get; set; } = string.Empty;

		[CommandArgument(1, "<on>")]
		public string On { get; set; } = "on";

		[CommandArgument(2, "<context>")]
		public string Context { get; set; } = string.Empty;

		[CommandOption("-t|--table")]
		public string? Table { get; set; }

		[CommandOption("-d|--description")]
		public string? Description { get; set; }

		[CommandOption("--aggregate-root")]
		public bool AggregateRoot { get; set; } = true;

		[CommandOption("--auditable")]
		public bool Auditable { get; set; } = true;
	}
	public sealed class AddEntityCommand
	: AsyncCommand<AddEntitySettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			AddEntitySettings settings,
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
					$"Context '{settings.Context}' not found!", "red");
				return -1;
			}

			if (forgeContext.Entities.ContainsKey(settings.Entity))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Entity '{settings.Entity}' already exists in context '{settings.Context}'.", "red");
				return -1;
			}

			forgeContext.Entities[settings.Entity] = new ForgeEntity
			{
				Table = settings.Table ?? (project.DefaultConventions.UsePluralizedTables ? Pluralize(settings.Entity) : settings.Entity),
				Description = settings.Description,
				AggregateRoot = settings.AggregateRoot,
				Auditable = settings.Auditable
			};

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Entity '{settings.Entity}' added on context '{settings.Context}'.");

			return 0;
		}
	}

}
