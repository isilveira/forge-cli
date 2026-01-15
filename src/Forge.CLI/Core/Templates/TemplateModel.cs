using Forge.CLI.Core.Artifacts;
using Forge.CLI.Models;

namespace Forge.CLI.Core.Templates
{
	public sealed class TemplateModel
	{
		public ForgeProject Project { get; init; } = null!;
		public ForgeContext Context { get; init; } = null!;
		public ForgeEntity Entity { get; init; } = null!;

		public ArtifactDescriptor Descriptor { get; init; } = null!;
	}
}
