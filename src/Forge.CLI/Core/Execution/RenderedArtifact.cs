using Forge.CLI.Core.Artifacts;

namespace Forge.CLI.Core.Execution
{
	public sealed class RenderedArtifact
	{
		public ArtifactDescriptor Descriptor { get; init; } = null!;
		public string Content { get; init; } = null!;
	}
}
