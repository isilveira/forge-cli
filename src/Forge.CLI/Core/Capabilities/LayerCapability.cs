namespace Forge.CLI.Core.Capabilities
{
	public sealed class LayerCapability
	{
		public Layer Layer { get; init; }
		public IReadOnlyCollection<ArtifactCapability> Artifacts { get; init; }
	}
}
