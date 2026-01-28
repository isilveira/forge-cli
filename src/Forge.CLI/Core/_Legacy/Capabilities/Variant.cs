namespace Forge.CLI.Core._Legacy.Capabilities
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
		
		// Web.Api
		Controller,

		// Web [SHARED]
		Form, Table, PageIndex, PageCreate, PageEdit,

		//Web.Razor
		Tab, Index,

		// Web.Blazor
		Filter, Dialog, Select, Page,

		// Shared / Generic
		Entity,
		None,
		All,
		New
	}
}
