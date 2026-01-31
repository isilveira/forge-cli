
namespace Forge.CLI.Models
{
	public sealed class ForgeContext
	{
		public string Description { get; set; } = "Default";
		public Dictionary<string, ForgeEntity> Entities { get; set; } = new();

        internal void Sharpen()
        {
            foreach (var (name, entity) in Entities)
            {
                entity.Sharpen();
			}
		}
    }

}
