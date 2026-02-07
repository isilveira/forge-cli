
using Forge.CLI.Shared.Helpers;

namespace Forge.CLI.Models
{
	public sealed class ForgeProperty
	{
		public string Type { get; set; } = default!;
		public bool Required { get; set; } = false;
		public int? Length { get; set; } = null;
		public bool HasMaxLength { get; set; } = false;
		public int? Precision { get; set; } = null;
		public int? Scale { get; set; } = null;
		public bool IsDisplay { get; set; } = false;

		internal void Sharpen()
		{
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
		}
	}
}
