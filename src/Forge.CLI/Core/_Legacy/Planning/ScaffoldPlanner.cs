using Forge.CLI.Core._Legacy.Capabilities;
using Forge.CLI.Core._Legacy.Target;
using Forge.CLI.Core.Target;
using Forge.CLI.Models;
using System.Collections.Generic;

namespace Forge.CLI.Core._Legacy.Planning
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
                .Resolve(request);

            var tasks = new List<ScaffoldTask>();
            
            foreach (var target in targets)
                tasks.AddRange(ResolveTasks(request, target));

            return new ScaffoldPlan
            {
                Tasks = tasks
            };
        }

        private static IReadOnlyCollection<ScaffoldTask> ResolveTasks(ScaffoldRequest request, ScaffoldTarget target)
        {
            var tasks = new List<ScaffoldTask>();

            var layers = new List<Layer>();
			
            if (request.Layer == Layer.All)
                layers.AddRange(CapabilityMatrix.Layers.Select(lc => lc.Layer));
            else
                layers.Add(request.Layer);

            foreach (var layer in layers)
            {
                var types = request.Type == ArtifactType.All
					? CapabilityMatrix.GetArtifacts(layer, target.Scope)
                    : CapabilityMatrix.SupportsArtifact(layer, target.Scope, request.Type)
                        ? [request.Type] : [];
                
                foreach (var type in types)
                {
                    var variants = ResolveVariants(layer, request, target, type);

                    foreach (var variant in variants)
                    {
                        tasks.Add(new ScaffoldTask
                        {
                            Layer = layer,
                            Type = type,
                            Variant = variant,
                            Target = target
                        });
                    }
                }
            }

            return tasks;
        }

        private static IReadOnlyCollection<Variant> ResolveVariants(Layer layer, ScaffoldRequest request, ScaffoldTarget target, ArtifactType type)
		{
			var allowed_variants = CapabilityMatrix.GetVariants(
				layer, target.Scope, type);

			var variants = allowed_variants
				.Where(v => (request.Variant == Variant.All && v != Variant.New) || v == request.Variant)
				.ToList();

			if (!allowed_variants.Any())
				variants = new List<Variant> { Variant.None };

            if(!variants.Any() && allowed_variants.All(v=> v == Variant.New))
            {
                return allowed_variants;
            }

			return variants;
		}
	}
}
