
namespace Forge.CLI.Models
{
	public sealed class ForgeContext
	{
		internal ForgeProject _project;
		internal string _contextName;
		public string Schema { get; set; } = "dbo";
		public string Description { get; set; } = "Default";
		public Dictionary<string, ForgeEntity> Entities { get; set; } = new();

        internal void Sharpen(ForgeProject project, string contextName, string defaultIdType)
        {
			_project = project;
			_contextName = contextName;
			if(string.IsNullOrWhiteSpace(Schema) || Schema.Equals("dbo", StringComparison.OrdinalIgnoreCase))
			{
				Schema = $"{_contextName}Db";
			}
			if(string.IsNullOrWhiteSpace(Description) || Description.Equals("Default", StringComparison.OrdinalIgnoreCase))
			{
				Description = $"{_contextName}DbContext that represents the schema '{Schema}'";
			}
			foreach (var (name, entity) in Entities)
            {
                entity.Sharpen(this, name, defaultIdType);
			}
		}
    }

}
