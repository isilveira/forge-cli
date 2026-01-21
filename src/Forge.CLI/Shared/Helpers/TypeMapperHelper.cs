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
		public static string DbMap(string type) => type switch
		{
			null => "NVARCHAR",
			"" => "NVARCHAR",
			"string" => "NVARCHAR",
			"Guid" => "UNIQUEIDENTIFIER",
			"datetime" => "DATETIME",
			"small" => "TYNEINT",
			"int" => "INT",
			"long" => "BIGINT",
			"decimal" => "DECIMAL",
			"bool" => "BIT",
			_ => type
		};
	}
}
