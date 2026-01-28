using Forge.CLI.Core._Legacy.Target;

namespace Forge.CLI.Core._Legacy.Capabilities
{
	public sealed class ArtifactCapability
	{
		public TargetScope Scope { get; init; }
		public ArtifactType Type { get; init; }
		public IReadOnlyCollection<Variant> Variants { get; init; }
	}
}
