using Forge.CLI.Core.Capabilities;
using Forge.CLI.Core.Target;

namespace Forge.CLI.Core.Planning
{
	public sealed class ScaffoldTask
	{
		public Layer Layer { get; init; }
		public ArtifactType Type { get; init; }
		public Variant? Variant { get; init; }

		public ScaffoldTarget Target { get; init; } = null!;
	}
}
