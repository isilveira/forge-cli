namespace Forge.CLI.Core.Artifacts.Results
{
	public sealed class ArtifactLoadResult
	{
		public bool IsValid => Errors.Count == 0;
		public ArtifactDefinition? Artifact { get; init; }
		public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
	}
}
