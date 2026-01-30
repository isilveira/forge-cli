using Forge.CLI.Core.Artifacts.Interfaces;

namespace Forge.CLI.Core.Artifacts
{
	public sealed class ArtifactRegistry : IArtifactRegistry
	{
		private readonly IReadOnlyCollection<ArtifactDescriptor> _all;
		private readonly Dictionary<string, ArtifactDescriptor> _byId;
		private readonly Dictionary<string, List<ArtifactDescriptor>> _byLayer;
		private readonly Dictionary<string, List<ArtifactDescriptor>> _byType;
		private readonly Dictionary<(string layer, string type), List<ArtifactDescriptor>> _byLayerAndType;

		public ArtifactRegistry(IEnumerable<ArtifactDescriptor> artifacts)
		{
			var list = artifacts.ToList();

			_all = list.AsReadOnly();

			_byId = list.ToDictionary(
				a => a.Id,
				StringComparer.OrdinalIgnoreCase);

			_byLayer = list
				.GroupBy(a => a.Layer, StringComparer.OrdinalIgnoreCase)
				.ToDictionary(
					g => g.Key,
					g => g.ToList(),
					StringComparer.OrdinalIgnoreCase);

			_byType = list
				.GroupBy(a => a.Type, StringComparer.OrdinalIgnoreCase)
				.ToDictionary(
					g => g.Key,
					g => g.ToList(),
					StringComparer.OrdinalIgnoreCase);

			_byLayerAndType = list
				.GroupBy(a => (a.Layer.ToLowerInvariant(), a.Type.ToLowerInvariant()))
				.ToDictionary(
					g => g.Key,
					g => g.ToList());
		}

		public IReadOnlyCollection<ArtifactDescriptor> All => _all;

		public ArtifactDescriptor GetById(string id)
		{
			if (_byId.TryGetValue(id, out var artifact))
				return artifact;

			throw new KeyNotFoundException($"Artifact not found: {id}");
		}

		public IReadOnlyCollection<ArtifactDescriptor> GetByLayer(string layer)
		{
			return _byLayer.TryGetValue(layer, out var list)
				? list
				: Array.Empty<ArtifactDescriptor>();
		}

		public IReadOnlyCollection<ArtifactDescriptor> GetByType(string type)
		{
			return _byType.TryGetValue(type, out var list)
				? list
				: Array.Empty<ArtifactDescriptor>();
		}

		public IReadOnlyCollection<ArtifactDescriptor> GetByLayerAndType(
			string layer,
			string type)
		{
			return _byLayerAndType.TryGetValue(
				(layer.ToLowerInvariant(), type.ToLowerInvariant()),
				out var list)
				? list
				: Array.Empty<ArtifactDescriptor>();
		}

		public IReadOnlyCollection<ArtifactDescriptor> GetVariants(
			string layer,
			string type)
		{
			return GetByLayerAndType(layer, type)
				.Where(a => a.Variant is not null)
				.ToList();
		}
	}
}