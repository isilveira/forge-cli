namespace Forge.CLI.Core._Legacy.Capabilities
{
	public sealed class LayerCapability
	{
		public Layer Layer { get; init; }
		public IReadOnlyCollection<ArtifactCapability> Artifacts { get; init; }
	}
}
