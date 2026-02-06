namespace Forge.CLI.Models
{
	public sealed class ForgeRelation
	{
		internal ForgeEntity _entity;
		public string Type { get; set; } = default!; // one-to-many, many-to-one
		public string Target { get; set; } = default!;
		public bool Required { get; set; } = false;
		internal void Sharpen(ForgeEntity entity)
		{
			_entity = entity;
		}
		public string GetTargetIdType()
		{
			var targetEntity = _entity._context.Entities.FirstOrDefault(e => e.Key == Target).Value;
			return targetEntity != null ? targetEntity.IdType : "Guid";
		}
	}
}
