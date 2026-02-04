using Forge.CLI.Core.Artifacts;
using Forge.CLI.Core.Artifacts.Interfaces;
using Forge.CLI.Core.Scaffolding.Conflict;
using Forge.CLI.Core.Scaffolding.Planning;
using Forge.CLI.Core.Templates;
using Forge.CLI.Models;
using Forge.CLI.Shared.Helpers;
using Scriban;
using Scriban.Runtime;

namespace Forge.CLI.Core.Scaffolding
{
	public sealed class ScaffoldPlanner
	{
		private readonly ForgeProject _project;
		private readonly IArtifactRegistry _registry;
		private readonly ITemplateRenderer _renderer;
		private readonly ConflictDetector _conflictDetector;
		private readonly string _projectRoot;

		public ScaffoldPlanner(
			ForgeProject project,
			IArtifactRegistry registry,
			ITemplateRenderer renderer,
			string? projectRoot = null,
			ConflictDetector? conflictDetector = null)
		{
			_project = project ?? throw new ArgumentNullException(nameof(project));
			_registry = registry ?? throw new ArgumentNullException(nameof(registry));
			_renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
			_projectRoot = projectRoot ?? Directory.GetCurrentDirectory();
			_conflictDetector = conflictDetector ?? new ConflictDetector();
		}

		public async Task<ScaffoldPlan> BuildAsync(
			ScaffoldRequest request,
			CancellationToken cancellationToken = default)
		{
			if (request is null) throw new ArgumentNullException(nameof(request));

			var plan = new ScaffoldPlan();

			var artifacts = FilterArtifacts(request).ToList();
			var targets = ResolveTargets(request).ToList();

			foreach (var artifact in artifacts)
			{
				foreach (var target in targets)
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (!artifact.Definition.Generation.Enabled)
					{
						plan.Add(new ScaffoldAction(
							ScaffoldActionType.Skip,
							filePath: artifact.SourceFile,
							reason: $"Artifact '{artifact.Id}' generation is disabled."));
						continue;
					}

					if (RequiresContext(artifact.Definition) && target.ContextName is null)
					{
						plan.Add(new ScaffoldAction(
							ScaffoldActionType.Skip,
							filePath: artifact.SourceFile,
							reason: $"Artifact '{artifact.Id}' requires a context target."));
						continue;
					}

					if (RequiresEntity(artifact.Definition) && target.EntityName is null)
					{
						plan.Add(new ScaffoldAction(
							ScaffoldActionType.Skip,
							filePath: artifact.SourceFile,
							reason: $"Artifact '{artifact.Id}' requires an entity target."));
						continue;
					}

					var variables = BuildVariables(request, target);

					var relativePath = RenderScriban(
						artifact.Definition.Generation.Target.Path,
						variables);

					var fileName = RenderScriban(
						artifact.Definition.Generation.Target.Filename,
						variables);

					var filePath = Path.Combine(_projectRoot, relativePath, fileName);

					// Marker detection is not implemented yet; we conservatively assume "no markers".
					var hasForgeMarkers = false;

					var actionType = _conflictDetector.Detect(
						filePath,
						request.OverwriteStrategy,
						hasForgeMarkers);

					if (actionType == ScaffoldActionType.Skip)
					{
						plan.Add(new ScaffoldAction(
							ScaffoldActionType.Skip,
							filePath,
							reason: "Skipped by overwrite strategy."));
						continue;
					}

					var model = BuildTemplateModel(artifact, request, target);
					var templateKey = artifact.Definition.Generation.Template.File;

					var content = await _renderer.RenderAsync(templateKey, model);

					plan.Add(new ScaffoldAction(
						actionType,
						filePath,
						content,
						actionType == ScaffoldActionType.Conflict ? "File exists and overwrite strategy requires manual decision." : null));
				}
			}

			return plan;
		}

		private IEnumerable<ArtifactDescriptor> FilterArtifacts(ScaffoldRequest request)
		{
			return _registry.All.Where(a =>
				(
					(request.Layer is null || a.Layer.Equals(request.Layer, StringComparison.OrdinalIgnoreCase))
					&& !_project.ScaffoldExceptions.Any(se => se.Equals($"{a.Layer}", StringComparison.OrdinalIgnoreCase))
				)
				&& (
					(request.Type is null || a.Type.Equals(request.Type, StringComparison.OrdinalIgnoreCase))
					&& !_project.ScaffoldExceptions.Any(se => se.Equals($"{a.Layer}.{a.Type}", StringComparison.OrdinalIgnoreCase))
				)
				&& (
					(
						(request.Variant is null && !string.Equals(a.Variant, "New", StringComparison.OrdinalIgnoreCase))
						|| string.Equals(a.Variant, request.Variant, StringComparison.OrdinalIgnoreCase)
					)
					&& !_project.ScaffoldExceptions.Any(se => se.Equals($"{a.Layer}.{a.Type}`.{a.Variant}", StringComparison.OrdinalIgnoreCase))
				)
			);
		}

		private IEnumerable<Target> ResolveTargets(ScaffoldRequest request)
		{
			var hasContextFilter = !string.IsNullOrWhiteSpace(request.ContextName);
			var hasEntityFilter = !string.IsNullOrWhiteSpace(request.EntityName);

			if (!hasContextFilter && !hasEntityFilter)
			{
				foreach (var (contextName, context) in _project.Contexts)
				{
					if (context.Entities.Any())
					{
						foreach (var entityName in context.Entities.Keys)
							yield return new Target(contextName, entityName);
					}
					else
					{
						yield return new Target(contextName, null);
					}
				}

				yield break;
			}

			if (hasContextFilter && hasEntityFilter)
			{
				var context = _project.Contexts[request.ContextName!];
				_ = context.Entities[request.EntityName!];
				yield return new Target(request.ContextName!, request.EntityName!);
				yield break;
			}

			if (hasContextFilter)
			{
				var context = _project.Contexts[request.ContextName!];
				if (context.Entities.Any())
				{
					foreach (var entityName in context.Entities.Keys)
						yield return new Target(request.ContextName!, entityName);
				}
				else
				{
					yield return new Target(request.ContextName!, null);
				}

				yield break;
			}

			// entity filter only: search across contexts
			foreach (var (contextName, context) in _project.Contexts)
			{
				if (context.Entities.ContainsKey(request.EntityName!))
					yield return new Target(contextName, request.EntityName!);
			}
		}

		private ScriptObject BuildVariables(ScaffoldRequest request, Target target)
		{
			var variables = new ScriptObject();

			variables["projectName"] = _project.Name;
			variables["name"] = request.Name;
			variables["contextName"] = target.ContextName ?? string.Empty;
			variables["entityName"] = target.EntityName ?? string.Empty;
			variables["entityCollection"] = target.EntityName is null
				? string.Empty
				: RazorHelper.Pluralize(target.EntityName);

			return variables;
		}

		private TemplateModel BuildTemplateModel(
			ArtifactDescriptor descriptor,
			ScaffoldRequest request,
			Target target)
		{
			var context = target.ContextName is not null && _project.Contexts.TryGetValue(target.ContextName, out var ctx)
				? ctx
				: new ForgeContext();

			var entity = target.EntityName is not null && context.Entities.TryGetValue(target.EntityName, out var ent)
				? ent
				: new ForgeEntity();

			return new TemplateModel
			{
				Project = _project,
				ContextName = target.ContextName ?? string.Empty,
				Context = context,
				EntityName = target.EntityName ?? string.Empty,
				Entity = entity,
				Name = request.Name,
				Descriptor = descriptor
			};
		}

		private static string RenderScriban(string template, ScriptObject variables)
		{
			var parsed = Template.Parse(template);
			if (parsed.HasErrors)
			{
				var errors = string.Join("; ", parsed.Messages.Select(m => m.Message));
				throw new InvalidOperationException($"Invalid Scriban template: {errors}");
			}

			var ctx = new TemplateContext();
			ctx.PushGlobal(variables);
			return parsed.Render(ctx);
		}

		private static bool RequiresContext(ArtifactDefinition definition)
			=> ContainsPlaceholder(definition, "contextName");

		private static bool RequiresEntity(ArtifactDefinition definition)
			=> ContainsPlaceholder(definition, "entityName") || ContainsPlaceholder(definition, "entityCollection");

		private static bool ContainsPlaceholder(ArtifactDefinition definition, string name)
		{
			var token1 = "{{" + name;
			var token2 = "{{ " + name;
			return definition.Generation.Target.Path.Contains(token1, StringComparison.OrdinalIgnoreCase)
			       || definition.Generation.Target.Path.Contains(token2, StringComparison.OrdinalIgnoreCase)
			       || definition.Generation.Target.Filename.Contains(token1, StringComparison.OrdinalIgnoreCase)
			       || definition.Generation.Target.Filename.Contains(token2, StringComparison.OrdinalIgnoreCase);
		}

		private sealed record Target(string? ContextName, string? EntityName);
	}
}
