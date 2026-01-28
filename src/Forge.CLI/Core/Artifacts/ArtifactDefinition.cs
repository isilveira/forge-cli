namespace Forge.CLI.Core.Artifacts
{
	public sealed class ArtifactDefinition
	{
		public ArtifactMetadata Artifact { get; init; } = default!;
		public string Layer { get; init; } = default!;
		public string Type { get; init; } = default!;
		public string? Variant { get; init; }
		public string? Description { get; init; }

		public ModelDefinition Model { get; init; } = default!;
		public GenerationDefinition Generation { get; init; } = default!;
		public ReadingDefinition? Reading { get; init; }
	}
}
