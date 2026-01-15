using Forge.CLI.Core.Capabilities;

namespace Forge.CLI.Core.Planning
{
	public sealed class ScaffoldRequest
	{
		public Layer Layer { get; init; }

		public ArtifactType? Type { get; init; }
		public Variant? Variant { get; init; }

		public string? ContextName { get; init; }
		public string? EntityName { get; init; }
	}
}
