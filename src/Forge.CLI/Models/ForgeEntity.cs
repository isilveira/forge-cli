
using Forge.CLI.Shared.Helpers;

namespace Forge.CLI.Models
{
	public sealed class ForgeEntity
	{
		internal ForgeContext _context;
		internal string _entityName;
		public string IdType { get; set; } = "Guid";
		public string? Description { get; set; } = default;
		public string Table { get; set; } = default;
		public bool AggregateRoot { get; set; } = true;
		public bool Auditable { get; set; } = true;
		public Dictionary<string, ForgeProperty> Properties { get; set; } = new();
		public Dictionary<string, ForgeRelation> Relations { get; set; } = new();

		internal void Sharpen(ForgeContext context, string entityName, string defaultIdType)
		{
			_context = context;
			_entityName = entityName;
			if (string.IsNullOrWhiteSpace(IdType))
			{
				IdType = defaultIdType;
			}
			IdType = TypeMapperHelper.Map(IdType);
			if (string.IsNullOrWhiteSpace(Table))
			{
				Table = _context._project.DefaultConventions.UsePluralizedTables ? ForgeHelper.Pluralize(entityName) : entityName;
			}
			if (string.IsNullOrWhiteSpace(Description))
			{
				Description = $"Entity that represents the table '{Table}'";
			}
			foreach (var (name, property) in Properties)
			{
				property.Sharpen(this, name);
			}
			foreach (var (name, relation) in Relations)
			{
				relation.Sharpen(this);
			}
		}
    }
}
