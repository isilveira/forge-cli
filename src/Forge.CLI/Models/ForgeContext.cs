
namespace Forge.CLI.Models
{
	public sealed class ForgeContext
	{
		internal ForgeProject _project;
		public string Description { get; set; } = "Default";
		public Dictionary<string, ForgeEntity> Entities { get; set; } = new();

        internal void Sharpen(ForgeProject project, string defaultIdType)
        {
			_project = project;
			foreach (var (name, entity) in Entities)
            {
                entity.Sharpen(this, defaultIdType);
			}
		}
    }

}
