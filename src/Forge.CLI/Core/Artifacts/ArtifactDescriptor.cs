namespace Forge.CLI.Core.Artifacts
{
	public sealed class ArtifactDescriptor
	{
		public string Id { get; init; } = default!;
		public string Layer { get; init; } = default!;
		public string Type { get; init; } = default!;
		public string? Variant { get; init; }

		public ArtifactDefinition Definition { get; init; } = default!;
		public string SourceFile { get; init; } = default!;
	}
}
