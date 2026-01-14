using BAYSOFT.Abstractions.Crosscutting.Pluralization;
using BAYSOFT.Abstractions.Crosscutting.Pluralization.Portuguese;
using Forge.CLI.Models;
using Forge.CLI.Scaffolding.Generators;
using Forge.CLI.Scaffolding.Types;
using Forge.CLI.Shared.Helpers;
using Forge.CLI.Tools;
using Spectre.Console.Cli;
using System.Threading;
using static Forge.CLI.Scaffolding.Types.ScaffoldTypes;

namespace Forge.CLI.Commands.Scaffold
{
	public sealed class ScaffoldEntitySettings : CommandSettings
	{
		[CommandArgument(0, "<entity>")]
		public string Entity { get; set; } = string.Empty;

		[CommandArgument(1, "<on>")]
		public string On { get; set; } = "on";

		[CommandArgument(2, "<context>")]
		public string Context { get; set; } = string.Empty;

		[CommandOption("--output <dir>")]
		public string? Output { get; set; }

		[CommandOption("--overwrite")]
		public bool Overwrite { get; set; } = false;

		[CommandOption("--type <type>")]
		public string? Type { get; set; } // class, dto, repository (para evoluir depois)
	}
	public sealed class ScaffoldEntityCommand : AsyncCommand<ScaffoldEntitySettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			ScaffoldEntitySettings settings,
			CancellationToken cancellationToken)
		{
			var loader = new ProjectLoader();

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
			
			var types = ScaffoldTypeParser.Parse(settings.Type);

			foreach (var type in types)
			{
				switch (type)
				{
					case EntityScaffoldType.Entity:
						await Generate(new EntityScaffoldGenerator());
						break;
				}
			}

			async Task<int> Generate(dynamic generator)
			{
				var files = generator.Generate(project, settings.Context, forgeContext, settings.Entity, entity);
				
				foreach(var file in files)
				{
					// Define output folder
					var outputDir = string.IsNullOrWhiteSpace(settings.Output)
						? file.Path
						: Path.GetFullPath(settings.Output);

					Directory.CreateDirectory(outputDir);

					// Arquivo de classe
					var classFile = $"{outputDir}/{file.Name}";
					if (File.Exists(classFile) && !settings.Overwrite && !AnsiConsoleHelper.SafeConfirm($"Are you sure you want to remove the entity {settings.Entity} from context '{settings.Context}'?"))
					{
						AnsiConsoleHelper.SafeMarkupLine(
							$"Skipped {file}", "yellow");

						return 0;
					}

					await File.WriteAllTextAsync(classFile, file.Content, cancellationToken);

					AnsiConsoleHelper.SafeMarkupLine(
						$"Generated {file}");
				}

				return 0;
			}

			return 0;
		}
	}
}
