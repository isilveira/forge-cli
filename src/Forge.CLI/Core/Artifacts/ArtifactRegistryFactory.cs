using Forge.CLI.Core.Artifacts.Interfaces;
using Forge.CLI.Core.Artifacts.Loaders;

namespace Forge.CLI.Core.Artifacts
{
	public static class ArtifactRegistryFactory
	{
		public static (IArtifactRegistry Registry, IReadOnlyList<string> Errors)
			Create(string projectRoot)
		{
			IArtifactLoader artifactLoader = new ArtifactYamlLoader();

			var discovery = new ArtifactDiscoveryService(artifactLoader);
			var discoveryResult = discovery.Discover(projectRoot);

			var registry = new ArtifactRegistry(discoveryResult.Artifacts);

			return (registry, discoveryResult.Errors);
		}
	}
}