namespace Forge.CLI.Core.Artifacts.Results
{
	public sealed class ArtifactDiscoveryResult
	{
		public IReadOnlyList<ArtifactDescriptor> Artifacts { get; init; } = [];
		public IReadOnlyList<string> Errors { get; init; } = [];
	}

}
