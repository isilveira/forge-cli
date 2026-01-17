using Forge.CLI.Core.Target;

namespace Forge.CLI.Core.Capabilities
{
	public sealed class ArtifactCapability
	{
		public TargetScope Scope { get; init; }
		public ArtifactType Type { get; init; }
		public IReadOnlyCollection<Variant> Variants { get; init; }
	}
}
