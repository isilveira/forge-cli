using Forge.CLI.Core.Artifacts;
using Forge.CLI.Models;

namespace Forge.CLI.Core.Templates
{
	public sealed class TemplateModel
	{
		public string ContextName { get; init; } = null!;
		public string EntityName { get; init; } = null!;
		public string Name { get; init; } = "New";

		public ForgeProject Project { get; init; } = null!;
		public ForgeContext Context { get; init; } = null!;
		public ForgeEntity Entity { get; init; } = null!;

		public ArtifactDescriptor Descriptor { get; init; } = null!;
	}
}

