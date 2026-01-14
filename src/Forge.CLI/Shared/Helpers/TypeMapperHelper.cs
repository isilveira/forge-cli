namespace Forge.CLI.Shared.Helpers
{
	public static class TypeMapperHelper
    {
		public static string Map(string type) => type switch
		{
			null => "string",
			"" => "string",
			"guid" => "Guid",
			"datetime" => "DateTime",
			_ => type
		};
	}
}
