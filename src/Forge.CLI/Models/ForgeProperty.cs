
using Forge.CLI.Shared.Helpers;

namespace Forge.CLI.Models
{
	public sealed class ForgeProperty
	{
		internal ForgeEntity _entity;
		internal string _propertyName;
		public string Type { get; set; } = default!;
		public bool Required { get; set; } = false;
		public int? Length { get; set; } = null;
		public bool HasMaxLength { get; set; } = false;
		public int? Precision { get; set; } = null;
		public int? Scale { get; set; } = null;
		public string? DbColumn { get; set; } = default!;
		public bool DisplayOnSelect { get; set; } = false;
		internal void Sharpen(ForgeEntity entity, string propertyName)
		{
			_entity = entity;
			_propertyName = propertyName;
			if (!TypeMapperHelper.HasLength(Type))
			{
				Length = null;
				HasMaxLength = false;
			}
			if (!TypeMapperHelper.HasPrecision(Type))
			{
				Precision = null;
			}
			if (!TypeMapperHelper.HasScale(Type))
			{
				Scale = null;
			}
			if (string.IsNullOrWhiteSpace(DbColumn))
			{
				DbColumn = _propertyName;
			}
		}
	}
}
