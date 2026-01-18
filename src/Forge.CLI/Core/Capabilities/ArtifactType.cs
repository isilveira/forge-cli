namespace Forge.CLI.Core.Capabilities
{
    public enum ArtifactType
    {
		All,

		Resource,

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
		DbContextWriter
	}
}
