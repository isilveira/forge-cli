using Forge.CLI.Core.Target;

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
							Scope = TargetScope.Entity,
							Type = ArtifactType.Command,
							Variants = [Variant.New, Variant.Post, Variant.Put, Variant.Patch, Variant.Delete]
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Entity,
							Type = ArtifactType.Query,
							Variants = [Variant.New, Variant.GetById, Variant.GetByFilter]
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Entity,
							Type = ArtifactType.Notification,
							Variants = [Variant.New, Variant.Post, Variant.Put, Variant.Patch, Variant.Delete]
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
							Scope = TargetScope.Entity,
							Type = ArtifactType.Entity,
							Variants = []
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Entity,
							Type = ArtifactType.Service,
							Variants = [Variant.New, Variant.Create, Variant.Update, Variant.Delete]
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Entity,
							Type = ArtifactType.Validation,
							Variants = [Variant.New, Variant.Entity, Variant.Create, Variant.Update, Variant.Delete]
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Entity,
							Type = ArtifactType.Specification,
							Variants = [Variant.New]
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Entity,
							Type = ArtifactType.EntityResource,
							Variants = [Variant.Resource, Variant.Designer, Variant.Culture]
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Context,
							Type = ArtifactType.ContextResource,
							Variants = [Variant.Resource, Variant.Designer, Variant.Culture]
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Context,
							Type = ArtifactType.IDbContextReader,
							Variants = []
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Context,
							Type = ArtifactType.IDbContextWriter,
							Variants = []
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Project,
							Type = ArtifactType.Resource,
							Variants = [Variant.Resource, Variant.Designer, Variant.Culture]
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Project,
							Type = ArtifactType.IService,
							Variants = [Variant.New]
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
							Scope = TargetScope.Entity,
							Type = ArtifactType.Mapping,
							Variants = []
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Context,
							Type = ArtifactType.DbContext,
							Variants = []
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Context,
							Type = ArtifactType.DbContextReader,
							Variants = []
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Context,
							Type = ArtifactType.DbContextWriter,
							Variants = []
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Project,
							Type = ArtifactType.Service,
							Variants = [Variant.New]
						}
					}
				},
				new LayerCapability
				{
					Layer = Layer.Middleware,
					Artifacts = new []
					{
						new ArtifactCapability
						{
							Scope = TargetScope.Project,
							Type = ArtifactType.DbContextConfigurations,
							Variants = []
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Project,
							Type = ArtifactType.DomainServicesConfigurations,
							Variants = []
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Project,
							Type = ArtifactType.ValidationsConfigurations,
							Variants = []
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Project,
							Type = ArtifactType.Configurations,
							Variants = []
						}
					}
				},
				new LayerCapability
				{
					Layer = Layer.Web,
					Artifacts = new []
					{
						new ArtifactCapability
						{
							Scope = TargetScope.Entity,
							Type = ArtifactType.Api,
							Variants = [Variant.Controller]
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Entity,
							Type = ArtifactType.React,
							Variants = [Variant.Index, Variant.Tab, Variant.Form, Variant.Table, Variant.PageIndex, Variant.PageCreate, Variant.PageEdit]
						},
						new ArtifactCapability
						{
							Scope = TargetScope.Entity,
							Type = ArtifactType.Blazor,
							Variants = [Variant.Filter, Variant.Dialog, Variant.Select, Variant.Page, Variant.Form, Variant.Table, Variant.PageIndex, Variant.PageCreate, Variant.PageEdit]
						},
					}
				}
			};

		public static LayerCapability GetLayer(Layer layer)
			=> Layers.Single(l => l.Layer == layer);
		public static bool SupportsArtifact(Layer layer, TargetScope scope, ArtifactType type)
			=> GetLayer(layer).Artifacts.Where(a => a.Scope == scope).Any(a => a.Type == type);
		public static IReadOnlyCollection<ArtifactType> GetArtifacts(Layer layer, TargetScope scope)
			=> GetLayer(layer).Artifacts.Where(a => a.Scope == scope).Select(a => a.Type).ToList();
		public static IReadOnlyCollection<Variant> GetVariants(Layer layer, TargetScope scope, ArtifactType type)
			=> GetLayer(layer).Artifacts.Where(a => a.Scope == scope).Single(a => a.Type == type).Variants;
		public static bool SupportsVariant(
			Layer layer,
			TargetScope scope,
			ArtifactType type,
			Variant variant)
		{
			var artifact = GetLayer(layer)
				.Artifacts
				.Where(a => a.Scope == scope)
				.Single(a => a.Type == type);

			return artifact.Variants.Contains(variant);
		}

	}

}
