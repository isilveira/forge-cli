namespace Forge.CLI.Core._Legacy.Capabilities
{
    public enum ArtifactType
    {
		All,

		Resource,
		DbContextConfigurations,
		DomainServicesConfigurations,
		ValidationsConfigurations,
		Configurations,

		IService,

		ContextResource,
		IDbContextReader,
		IDbContextWriter,

		Entity,

		EntityResource,

		Service,
		Specification,
		Validation,

		Command,
		Query,
		Notification,

		Mapping,
		DbContext,
		DbContextReader,
		DbContextWriter,

		Api, Blazor, React
	}
}
