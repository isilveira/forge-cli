using Forge.CLI.Core._Legacy.Capabilities;
using Forge.CLI.Core._Legacy.Target;

namespace Forge.CLI.Core._Legacy.Planning
{
	public sealed class ScaffoldTask
	{
		public Layer Layer { get; init; }
		public ArtifactType Type { get; init; }
		public Variant? Variant { get; init; }

		public ScaffoldTarget Target { get; init; } = null!;
	}
}
