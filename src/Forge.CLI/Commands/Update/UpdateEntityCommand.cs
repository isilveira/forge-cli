using Forge.CLI.Shared.Helpers;
using Forge.CLI.Tools;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Update
{
	// Settings
	public sealed class UpdateEntitySettings : CommandSettings
	{
		[CommandArgument(0, "<entity>")]
		public string Entity { get; set; } = string.Empty;

		[CommandArgument(1, "<on>")]
		public string On { get; set; } = "on";

		[CommandArgument(2, "<context>")]
		public string Context { get; set; } = string.Empty;

		[CommandOption("--name")]
		public string? NewName { get; set; }

		[CommandOption("-d|--description")]
		public string? Description { get; set; } = null;

		[CommandOption("--aggregate-root")]
		public bool? AggregateRoot { get; set; } = null;

		[CommandOption("--auditable")]
		public bool? Auditable { get; set; } = null;
	}
	public sealed class UpdateEntityCommand : AsyncCommand<UpdateEntitySettings>
	{
		public override async Task<int> ExecuteAsync(CommandContext context, UpdateEntitySettings settings, CancellationToken cancellationToken)
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
					$"Entity '{settings.Entity}' not found.", "red");
				return -1;
			}

			if (settings.Description is not null)
			{
				entity.Description = settings.Description;
				AnsiConsoleHelper.SafeMarkupLine(
					$"Entity '{settings.Entity}' description updated.");
			}

			if (settings.AggregateRoot.HasValue)
			{
				entity.AggregateRoot = settings.AggregateRoot.Value;
				AnsiConsoleHelper.SafeMarkupLine(
					$"Entity '{settings.Entity}' aggregate root set to '{settings.AggregateRoot.Value}'.");
			}

			if (settings.Auditable.HasValue)
			{
				entity.Auditable = settings.Auditable.Value;
				AnsiConsoleHelper.SafeMarkupLine(
					$"Entity '{settings.Entity}' auditable set to '{settings.Auditable.Value}'.");
			}

			if (!string.IsNullOrWhiteSpace(settings.NewName))
			{
				forgeContext.Entities.Remove(settings.Entity);
				forgeContext.Entities[settings.NewName] = entity;

				AnsiConsoleHelper.SafeMarkupLine(
					$"Entity '{settings.Entity}' renamed to '{settings.NewName}'.");
			}

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Entity '{settings.Entity}' updated'.");

			return 0;
		}
	}
}