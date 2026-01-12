using Forge.CLI.Shared.Helpers;
using Forge.CLI.Tools;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Update
{
	// Settings
	public sealed class UpdatePropertySettings : CommandSettings
	{
		[CommandArgument(0, "<property>")]
		public string Property { get; set; } = string.Empty;

		[CommandArgument(1, "<on>")]
		public string On { get; set; } = "on";

		[CommandArgument(2, "<entity>")]
		public string Entity { get; set; } = string.Empty;

		[CommandArgument(3, "<onContext>")]
		public string Context { get; set; } = string.Empty;

		[CommandOption("--name")]
		public string? NewName { get; set; } = null;

		[CommandOption("--type")]
		public string? Type { get; set; } = null;

		[CommandOption("--required")]
		public bool? Required { get; set; } = null;

		[CommandOption("--length")]
		public int? Length { get; set; } = null;

		[CommandOption("--precision")]
		public int? Precision { get; set; } = null;

		[CommandOption("--scale")]
		public int? Scale { get; set; } = null;
	}
	public sealed class UpdatePropertyCommand : AsyncCommand<UpdatePropertySettings>
	{
		public override async Task<int> ExecuteAsync(CommandContext context, UpdatePropertySettings settings, CancellationToken cancellationToken)
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

			if (!entity.Properties.TryGetValue(settings.Property, out var prop))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Property '{settings.Property}' not found in entity '{settings.Entity}'.", "red");
				return -1;
			}

			if (!string.IsNullOrWhiteSpace(settings.Type))
			{
				prop.Type = settings.Type;
				
				AnsiConsoleHelper.SafeMarkupLine(
					$"Property '{settings.Property}' type updated to '{settings.Type}'.");
			}

			if (settings.Required.HasValue)
			{
				prop.Required = settings.Required.Value;
				
				AnsiConsoleHelper.SafeMarkupLine(
					$"Property '{settings.Property}' required set to '{settings.Required.Value}'.");
			}

			if (settings.Length.HasValue)
			{
				prop.Length = settings.Length.Value;
				
				AnsiConsoleHelper.SafeMarkupLine(
					$"Property '{settings.Property}' length updated to '{settings.Length.Value}'.");
			}

			if (settings.Precision.HasValue)
			{
				prop.Precision = settings.Precision.Value;
				
				AnsiConsoleHelper.SafeMarkupLine(
					$"Property '{settings.Property}' precision updated to '{settings.Precision.Value}'.");
			}

			if (settings.Scale.HasValue)
			{
				prop.Scale = settings.Scale.Value;
				
				AnsiConsoleHelper.SafeMarkupLine(
					$"Property '{settings.Property}' scale updated to '{settings.Scale.Value}'.");
			}

			if (settings.NewName is not null)
			{
				entity.Properties.Remove(settings.Property);
				entity.Properties[settings.NewName] = prop;
				
				AnsiConsoleHelper.SafeMarkupLine(
					$"Property '{settings.Property}' renamed to '{settings.NewName}'.");
			}

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Property '{settings.Property}' updated.");

			return 0;
		}
	}
}