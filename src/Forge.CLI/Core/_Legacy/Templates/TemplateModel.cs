using Forge.CLI.Core._Legacy.Artifacts;
using Forge.CLI.Models;

namespace Forge.CLI.Core._Legacy.Templates
{
	public sealed class TemplateModel
	{
		public string ContextName { get; set; }
        public string EntityName { get; set; }
        public ForgeProject Project { get; init; } = null!;
		public ForgeContext Context { get; init; } = null!;
		public ForgeEntity Entity { get; init; } = null!;

		public ArtifactDescriptor Descriptor { get; init; } = null!;
	}
}
