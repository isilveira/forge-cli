namespace Forge.CLI.Core.Artifacts.Interfaces
{
	public interface IArtifactRegistry
	{
		IReadOnlyCollection<ArtifactDescriptor> All { get; }

		ArtifactDescriptor GetById(string id);

		IReadOnlyCollection<ArtifactDescriptor> GetByLayer(string layer);

		IReadOnlyCollection<ArtifactDescriptor> GetByType(string type);

		IReadOnlyCollection<ArtifactDescriptor> GetByLayerAndType(
			string layer,
			string type);

		IReadOnlyCollection<ArtifactDescriptor> GetVariants(
			string layer,
			string type);
	}
}
