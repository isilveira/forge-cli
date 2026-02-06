
namespace Forge.CLI.Models
{
	public sealed class ForgeEntity
	{
		internal ForgeContext _context;
		public string IdType { get; set; } = "Guid";
		public string? Description { get; set; } = default;
		public bool AggregateRoot { get; set; } = true;
		public bool Auditable { get; set; } = true;
		public Dictionary<string, ForgeProperty> Properties { get; set; } = new();
		public Dictionary<string, ForgeRelation> Relations { get; set; } = new();

		internal void Sharpen(ForgeContext context, string defaultIdType)
		{
			_context = context;
			if (string.IsNullOrWhiteSpace(IdType))
			{
				IdType = defaultIdType;
			}
			foreach (var (name, property) in Properties)
			{
				property.Sharpen();
			}
			foreach (var (name, relation) in Relations)
			{
				relation.Sharpen(this);
			}
		}
    }
}
