using Forge.CLI.Core._Legacy.Capabilities;
using Forge.CLI.Core._Legacy.Target;

namespace Forge.CLI.Core._Legacy.Artifacts
{
	public sealed class ArtifactDescriptor
	{
		public Layer Layer { get; init; }
		public ArtifactType Type { get; init; }
		public Variant? Variant { get; init; }

		public TargetScope Scope { get; init; }

		public string RelativePath { get; init; } = null!;
		public string FileName { get; init; } = null!;

		public string TemplateKey { get; init; } = null!;

		public ScaffoldTarget Target { get; init; } = null!;
	}
}
