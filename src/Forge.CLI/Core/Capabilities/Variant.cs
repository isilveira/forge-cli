namespace Forge.CLI.Core.Capabilities
{
	public enum Variant
	{
		// Domain
		Create,
		Update,
		Delete,

		// Application
		Post,
		Put,
		Patch,
		GetById,
		GetByFilter,

		Resource,
		Designer,
		Culture,

		// Shared / Generic
		Entity,
		None,
		All,
		New
	}
}
