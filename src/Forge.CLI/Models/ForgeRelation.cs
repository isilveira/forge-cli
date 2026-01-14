namespace Forge.CLI.Models
{
	public sealed class ForgeRelation
	{
		public string Type { get; set; } = default!; // one-to-many, many-to-one
		public string Target { get; set; } = default!;
		public bool Required { get; set; } = false;
	}

}
