namespace Forge.CLI.Core.Capabilities
{
	public static class CapabilityMatrix
	{
		public static readonly IReadOnlyCollection<LayerCapability> Layers =
			new List<LayerCapability>
			{
				new LayerCapability
				{
					Layer = Layer.Application,
					Artifacts = new[]
					{
						new ArtifactCapability
						{
							Type = ArtifactType.Command,
							Variants = new[]
							{
								Variant.Post,
								Variant.Put,
								Variant.Patch,
								Variant.Delete,
								Variant.New,
							}
						},
						new ArtifactCapability
						{
							Type = ArtifactType.Query,
							Variants = new[]
							{
								Variant.GetById,
								Variant.GetByFilter,
								Variant.New
							}
						},
						new ArtifactCapability
						{
							Type = ArtifactType.Notification,
							Variants = new[]
							{
								Variant.Post,
								Variant.Put,
								Variant.Patch,
								Variant.Delete,
								Variant.New,
							}
						}
					}
				},
				new LayerCapability
				{
					Layer = Layer.Domain,
					Artifacts = new[]
					{
						new ArtifactCapability
						{
							Type = ArtifactType.Entity,
							Variants = Array.Empty<Variant>()
						},
						new ArtifactCapability
						{
							Type = ArtifactType.Service,
							Variants = new[]
							{
								Variant.Create,
								Variant.Update,
								Variant.Delete,
								Variant.New
							}
						},
						new ArtifactCapability
						{
							Type = ArtifactType.Validation,
							Variants = new[]
							{
								Variant.Entity,
								Variant.Create,
								Variant.Update,
								Variant.Delete,
								Variant.New
							}
						},
						new ArtifactCapability
						{
							Type = ArtifactType.Specification,
							Variants = new[]
							{
								Variant.New
							}
						},
						new ArtifactCapability
						{
							Type = ArtifactType.EnityResource,
							Variants = Array.Empty<Variant>()
						},
						new ArtifactCapability
						{
							Type = ArtifactType.ContextResource,
							Variants = Array.Empty<Variant>()
						},
						new ArtifactCapability
						{
							Type = ArtifactType.Resource,
							Variants = Array.Empty<Variant>()
						}
					}
				},
				new LayerCapability
				{
					Layer = Layer.Infrastructure,
					Artifacts = new[]
					{
						new ArtifactCapability
						{
							Type = ArtifactType.Mapping,
							Variants = Array.Empty<Variant>()
						},
						new ArtifactCapability
						{
							Type = ArtifactType.DbContext,
							Variants = Array.Empty<Variant>()
						}
					}
				}
			};

		public static LayerCapability GetLayer(Layer layer)
			=> Layers.Single(l => l.Layer == layer);
		public static bool SupportsArtifact(Layer layer, ArtifactType type)
			=> GetLayer(layer).Artifacts.Any(a => a.Type == type);
		public static IReadOnlyCollection<ArtifactType> GetArtifacts(Layer layer)
			=> GetLayer(layer).Artifacts.Select(a => a.Type).ToList();
		public static IReadOnlyCollection<Variant>GetVariants(Layer layer, ArtifactType type)
			=> GetLayer(layer).Artifacts.Single(a => a.Type == type).Variants;
		public static bool SupportsVariant(
			Layer layer,
			ArtifactType type,
			Variant variant)
		{
			var artifact = GetLayer(layer).Artifacts
				.Single(a => a.Type == type);

			return artifact.Variants.Contains(variant);
		}

	}

}
