using Forge.CLI.Core.Artifacts;
using Forge.CLI.Core.Capabilities;
using Forge.CLI.Core.Execution;
using Forge.CLI.Core.Planning;
using Forge.CLI.Core.Templates;
using Forge.CLI.Core.Templates.Renderers;
using Forge.CLI.Models;
using Forge.CLI.Persistence;
using Scriban;
using Scriban.Runtime;
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
		public string? Name { get; set; }

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

			// 2. Parse enums
			var request = BuildRequest(settings);

			// 3. Build plan
			var planner = new ScaffoldPlanner(project);
			var plan = planner.Build(request);

			// 4. Resolve descriptors
			var descriptorResolver = new ArtifactDescriptorResolver();
			var descriptors = plan.Tasks
				.Select(descriptorResolver.Resolve)
				.ToList();

			// 5. Render templates
			var renderer = BuildRenderer(project);

			var renderTasks = descriptors
				.Select(d => Render(d, project, renderer))
				.ToList();

			await Task.WhenAll(renderTasks);

			var rendered = renderTasks.Select(t => t.Result).ToList();

			// 6. Execute
			Execute(rendered, settings);

			return 0;
		}
		private static ScaffoldRequest BuildRequest(
			ScaffoldSettings settings)
		{
			return new ScaffoldRequest
			{
				Layer = settings.Layer is null
					? Layer.All
					:Enum.Parse<Layer>(
						settings.Layer, true),

				Type = settings.Type is null
					? ArtifactType.All
					: Enum.Parse<ArtifactType>(
						settings.Type, true),

				Variant = settings.Variant is null
					? Variant.All
					: Enum.Parse<Variant>(
						settings.Variant, true),

				EntityName = settings.Entity,
				ContextName = settings.Context,

				Name = settings.Name,

				WhatIf = settings.WhatIf,
				Force = settings.Force,
				Yes = settings.Yes
			};
		}
		private static async Task<RenderedArtifact> Render(
			ArtifactDescriptor descriptor,
			ForgeProject project,
			ITemplateRenderer renderer)
		{
			var templateResolver =
				new TemplateResolver(new TemplateLoader().Load(descriptor));

			var template =
				templateResolver.Resolve(descriptor.TemplateKey);

			var model = new TemplateModelBuilder(project)
				.Build(descriptor);

			var content = await renderer.RenderAsync(template, model);

			return new RenderedArtifact
			{
				Descriptor = descriptor,
				Content = content
			};
		}
		private static void Execute(
			IReadOnlyCollection<RenderedArtifact> artifacts,
			ScaffoldSettings settings)
		{
			var options = new ExecutionOptions
			{
				WhatIf = settings.WhatIf,
				Force = settings.Force,
				ConfirmEach = !settings.Yes
			};

			var executor = new ScaffoldExecutor(
				new PhysicalFileSystem(),
				new PathResolver(Directory.GetCurrentDirectory()),
				new OverwritePolicy(options));

			executor.Execute(artifacts, options);
		}
		private static ITemplateRenderer BuildRenderer(
			ForgeProject project)
		{
			return new RazorTemplateRenderer();
		}
	}
}
