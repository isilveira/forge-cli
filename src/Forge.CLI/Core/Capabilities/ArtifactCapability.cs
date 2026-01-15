namespace Forge.CLI.Core.Capabilities
{
	public sealed class ArtifactCapability
	{
		public ArtifactType Type { get; init; }
		public IReadOnlyCollection<Variant> Variants { get; init; }
	}
}
