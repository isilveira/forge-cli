namespace Forge.CLI.Models
{
	public sealed class ForgeProject
	{
		public string SchemaVersion { get; set; } = "1.0";
		public string Name { get; set; } = default!;
		public string DefaultIdType { get; set; } = "Guid";
		public bool UseSealedClasses { get; set; } = true;
		public bool UseVirtualCollections { get; set; } = false;
		public string Tab { get; set; } = "    ";
		public Conventions DefaultConventions { get; set; } = new();
		public Dictionary<string, ForgeContext> Contexts { get; init; } = new();
		public List<string> ScaffoldExceptions { get; init; } = new();
		internal void Sharpen()
		{
			foreach(var (name, context) in Contexts)
			{
				context.Sharpen(this, name, DefaultIdType);
			}
		}
	}

    public sealed class  Conventions
    {
		public bool UsePluralizedTables { get; set; } = true;
		public string DefaultProject { get; set; } = "{projectName}";
		public string DefaultProjectPath { get; set; } = "{projectName}";
		public string DefaultContext { get; set; } = "{contextName}";
		public string DefaultContextPath { get; set; } = "{contextName}";
		public string DefaultEntity { get; set; } = "{entityName}";
		public string DefaultEntityPath { get; set; } = "{entityName}";
		public bool DefaultEntityPluralized { get; set; } = true;
    }
}
