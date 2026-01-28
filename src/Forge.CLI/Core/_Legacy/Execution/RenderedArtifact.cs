using Forge.CLI.Core._Legacy.Artifacts;

namespace Forge.CLI.Core._Legacy.Execution
{
	public sealed class RenderedArtifact
	{
		public ArtifactDescriptor Descriptor { get; init; } = null!;
		public string Content { get; init; } = null!;
	}
}
