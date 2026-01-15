using Forge.CLI.Core.Capabilities;
using Forge.CLI.Core.Target;
using Forge.CLI.Models;

namespace Forge.CLI.Core.Planning
{
	public sealed class ScaffoldPlanner
	{
		private readonly ForgeProject _project;

		public ScaffoldPlanner(ForgeProject project)
		{
			_project = project;
		}

		public ScaffoldPlan Build(ScaffoldRequest request)
		{

			var targets = new TargetResolver(_project)
				.Resolve(request.EntityName, request.ContextName);

			var types = request.Type.HasValue
				? new[] { request.Type.Value }
				: CapabilityMatrix.GetArtifacts(request.Layer);

			var tasks = new List<ScaffoldTask>();

			foreach (var target in targets)
			{
				foreach (var type in types)
				{
					var variants = ResolveVariants(request, type);

					foreach (var variant in variants)
					{
						tasks.Add(new ScaffoldTask
						{
							Layer = request.Layer,
							Type = type,
							Variant = variant,
							Target = target
						});
					}
				}
			}
			
			return new ScaffoldPlan
			{
				Tasks = tasks
			};
		}

		private static IReadOnlyCollection<Variant> ResolveVariants(ScaffoldRequest request, ArtifactType type)
		{
			var variants = CapabilityMatrix.GetVariants(
				request.Layer, type);

			if (request.Variant.HasValue)
				variants = variants
					.Where(v => v == request.Variant.Value)
					.ToList();

			if (!variants.Any())
				variants = new List<Variant> { Variant.None };

			return variants;
		}
	}
}
