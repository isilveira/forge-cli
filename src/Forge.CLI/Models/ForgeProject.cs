namespace Forge.CLI.Models
{
	public sealed class ForgeProject
	{
		public string SchemaVersion { get; set; } = "1.0";
		public string Name { get; set; } = default!;
		public Dictionary<string, ForgeContext> Contexts { get; init; } = new();
	}
}
