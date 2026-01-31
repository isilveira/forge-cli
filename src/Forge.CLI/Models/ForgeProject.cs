namespace Forge.CLI.Models
{
	public sealed class ForgeProject
	{
		public string SchemaVersion { get; set; } = "1.0";
		public string Name { get; set; } = default!;
		public string IdType { get; set; } = "Guid";
		public string Tab { get; set; } = "    ";
		public Dictionary<string, ForgeContext> Contexts { get; init; } = new();

		internal void Sharpen()
		{
			foreach(var (name, context) in Contexts)
			{
				context.Sharpen();
			}
		}
	}
}
