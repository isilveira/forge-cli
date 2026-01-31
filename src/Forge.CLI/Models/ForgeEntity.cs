
namespace Forge.CLI.Models
{
	public sealed class ForgeEntity
	{
		public string? Description { get; set; } = default;
		public bool AggregateRoot { get; set; } = true;
		public bool Auditable { get; set; } = true;
		public Dictionary<string, ForgeProperty> Properties { get; set; } = new();
		public Dictionary<string, ForgeRelation> Relations { get; set; } = new();

		internal void Sharpen()
		{
			foreach (var (name, property) in Properties)
			{
				property.Sharpen();
			}
		}
    }
}
