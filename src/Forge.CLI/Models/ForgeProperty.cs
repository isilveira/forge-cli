namespace Forge.CLI.Models
{
	public sealed class ForgeProperty
	{
		public string Type { get; set; } = default!;
		public bool Required { get; set; } = false;
		public int? Length { get; set; } = null;
		public int? Precision { get; set; } = null;
		public int? Scale { get; set; } = null;
	}
}
