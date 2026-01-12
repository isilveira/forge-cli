using BAYSOFT.Abstractions.Crosscutting.Pluralization;
using BAYSOFT.Abstractions.Crosscutting.Pluralization.Portuguese;
using Forge.CLI.Models;
using Forge.CLI.Shared.Helpers;
using Forge.CLI.Tools;
using Spectre.Console.Cli;

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

			// Define output folder
			var outputDir = string.IsNullOrWhiteSpace(settings.Output)
				? Path.Combine(Directory.GetCurrentDirectory(), settings.Context, "Entities")
				: Path.GetFullPath(settings.Output);

			Directory.CreateDirectory(outputDir);

			// Arquivo de classe
			var classFile = Path.Combine(outputDir, $"{settings.Entity}.cs");
			if (File.Exists(classFile) && !settings.Overwrite)
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"File '{classFile}' already exists. Use --overwrite to replace.", "yellow");
				return 0;
			}

			var content = GenerateClass(project, settings.Context, forgeContext, settings.Entity, entity);
			
			await File.WriteAllTextAsync(classFile, content, cancellationToken);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Entity '{settings.Entity}' scaffolded at '{classFile}'.");
			
			return 0;
		}

		private string GenerateClass(ForgeProject project, string contextName, ForgeContext context, string entityName, ForgeEntity entity)
		{
			var pluralizer = Pluralizer
				.GetInstance()
				.AddPortuguesePluralizer();

			var collection = pluralizer.Pluralize(entityName, "pt-BR");

			var filePath = $"src/{project.Name}.Core.Domain/{contextName}/Aggregates/{collection}/Entities/{entityName}.cs";

			var properties = string.Join(Environment.NewLine,
				entity.Properties.Select(p => $"        public {MapType(p.Value.Type)} {p.Key} {{ get; set; }}"));

			var relations = string.Join(Environment.NewLine, entity.Relations.Select(r =>
			{
				var target = r.Key;
				var type = r.Value.Type;
				if (type.Contains("many"))
					return $"        // Relation: {type} -> {target}\n        public List<{target}> {target}s {{ get; set; }} = new();";
				else
					return $"        // Relation: {type} -> {target}\n        public {target} {target} {{ get; set; }}";
			}));

			return @$"using System;
using System.Collections.Generic;

namespace {project.Name}.Core.Domain.{contextName}.Aggregates.{collection}.Entities
{{
	public class {entityName}
	{{
{properties}

{relations}
    }}
}}";
		}

		private string MapType(string type) => type switch
		{
			"string" => "string",
			"guid" => "Guid",
			"decimal" => "decimal",
			"datetime" => "DateTime",
			_ => "object"
		};
	}
}
