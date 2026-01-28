using Forge.CLI.Core._Legacy.Capabilities;

namespace Forge.CLI.Core._Legacy.Planning
{
	public sealed class ScaffoldRequest
	{
		public Layer Layer { get; init; }

		public ArtifactType Type { get; init; }
		public Variant Variant { get; init; }

		public string? ContextName { get; init; }
		public string? EntityName { get; init; }
		public bool All { get; init; }
		public string? Name { get; init; }
		public bool WhatIf { get; init; }
		public bool Force { get; init; }
		public bool Yes { get; init; }
	}
}
