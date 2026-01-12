using Forge.CLI.Models;
using Forge.CLI.Shared.Helpers;
using Forge.CLI.Tools;
using Spectre.Console.Cli;

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
