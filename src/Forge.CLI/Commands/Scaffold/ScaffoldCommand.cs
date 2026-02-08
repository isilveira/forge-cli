using Forge.CLI.Core.Artifacts;
using Forge.CLI.Core.Scaffolding.Conflict;
using Forge.CLI.Core.Scaffolding.Execution;
using Forge.CLI.Core.Scaffolding.Planning;
using Forge.CLI.Core.Templates;
using Forge.CLI.Core.Templates.Renderers;
using Forge.CLI.Persistence;
using Forge.CLI.Shared.Helpers;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Scaffold
{
	public sealed class ScaffoldSettings : CommandSettings
	{
		[CommandArgument(0, "[layer]")]
		public string? Layer { get; set; } = null!;

		[CommandArgument(1, "[type]")]
		public string? Type { get; set; }

		[CommandArgument(2, "[variant]")]
		public string? Variant { get; set; }
		
		[CommandOption("-c|--context <CONTEXT>")]
		public string? Context { get; set; }

		[CommandOption("-e|--entity <ENTITY>")]
		public string? Entity { get; set; }

		[CommandOption("-n|--name <NAME>")]
		public string? Name { get; set; } = "New";

		[CommandOption("--what-if")]
		public bool WhatIf { get; set; }

		[CommandOption("--force")]
		public bool Force { get; set; }

		[CommandOption("--yes")]
		public bool Yes { get; set; }
	}
	public sealed class ScaffoldCommand
	: AsyncCommand<ScaffoldSettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			ScaffoldSettings settings,
			CancellationToken cancellationToken)
		{
			// 1. Load project
			var project = new ProjectLoader().TryLoad();
			if (project is null)
			{
				AnsiConsoleHelper.SafeMarkupLine(
					"Projeto não inicializado. Execute `forge init project` antes de scaffold.",
					"red");
				return 1;
			}

			// 2. Build registry (YAML artifacts)
			var (registry, discoveryErrors) = ArtifactRegistryFactory.Create(
				Directory.GetCurrentDirectory());

			if (discoveryErrors.Any())
			{
				foreach (var err in discoveryErrors)
					AnsiConsoleHelper.SafeMarkupLine(err, "yellow");
			}

			// 3. Plan (new pipeline)
			var request = BuildRequest(settings);
			var renderer = BuildRenderer();
			var planner = new Forge.CLI.Core.Scaffolding.ScaffoldPlanner(
				project,
				registry,
				renderer,
				projectRoot: Directory.GetCurrentDirectory());

			var plan = await planner.BuildAsync(request, cancellationToken);

			// 4. Execute
			IScaffoldExecutor executor = settings.WhatIf
				? new DryRunExecutor()
				: new FileSystemExecutor();

			var execOptions = new ScaffoldExecutionOptions
			{
				ConfirmEach = !settings.Yes,
				TreatConflictsAsError = !settings.Force
			};

			var result = await executor.ExecuteAsync(plan, execOptions, cancellationToken);

			if (result.Conflicts > 0 && execOptions.TreatConflictsAsError)
				return 1;

			return 0;
		}
		private static ScaffoldRequest BuildRequest(
			ScaffoldSettings settings)
		{
			return new ScaffoldRequest
			{
				Layer = settings.Layer,
				Type = settings.Type,
				Variant = settings.Variant,
				ContextName = settings.Context,
				EntityName = settings.Entity,
				Name = settings.Name ?? "New",
				OverwriteStrategy = settings.Force
					? OverwriteStrategy.Always
					: OverwriteStrategy.MarkersOnly
			};
		}
		private static ITemplateRenderer BuildRenderer()
		{
			return new RazorTemplateRenderer();
		}
	}
}
