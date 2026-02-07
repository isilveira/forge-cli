namespace Forge.CLI.Core.SqlLoading
{
	/// <summary>
	/// Mapeia tipos SQL (script de migrations) para tipos do modelo Forge.
	/// Suporta tipos comuns do SQL Server (Entity Framework migrations).
	/// </summary>
	public static class SqlTypeMapper
	{
		/// <summary>
		/// Converte o tipo SQL para o tipo Forge (string usado em ForgeProperty.Type).
		/// </summary>
		public static string ToForgeType(string sqlType, int? length = null, int? precision = null, int? scale = null)
		{
			var type = sqlType.ToLowerInvariant();

			return type switch
			{
				"uniqueidentifier" => "Guid",
				"nvarchar" or "varchar" or "nchar" or "char" => "string",
				"int" => "int",
				"bigint" => "long",
				"smallint" or "tinyint" => "small",
				"decimal" or "numeric" => "decimal",
				"float" or "real" => "decimal",
				"bit" => "bool",
				"datetime" or "datetime2" or "date" or "smalldatetime" => "datetime",
				"rowversion" or "timestamp" => "byte[]",
				_ => sqlType
			};
		}

		/// <summary>
		/// Indica se o tipo Forge suporta Length (ex: string).
		/// </summary>
		public static bool HasLength(string forgeType)
		{
			return forgeType.Equals("string", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Indica se o tipo Forge suporta Precision (ex: decimal).
		/// </summary>
		public static bool HasPrecision(string forgeType)
		{
			return forgeType.Equals("decimal", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Indica se o tipo Forge suporta Scale (ex: decimal).
		/// </summary>
		public static bool HasScale(string forgeType)
		{
			return forgeType.Equals("decimal", StringComparison.OrdinalIgnoreCase);
		}
	}
}
