namespace Forge.CLI.Core.SqlLoading
{
	/// <summary>
	/// Modelo em memória extraído de um script SQL de criação de tabelas
	/// (estilo Entity Framework migrations).
	/// </summary>
	public sealed class ParsedSqlModel
	{
		/// <summary>
		/// Tabelas extraídas do script (chave = nome da tabela no SQL).
		/// </summary>
		public List<ParsedTable> Tables { get; } = [];
	}

	/// <summary>
	/// Tabela parseada (CREATE TABLE).
	/// </summary>
	public sealed class ParsedTable
	{
		/// <summary>
		/// Nome do schema (ex: dbo) ou null.
		/// </summary>
		public string? Schema { get; set; }

		/// <summary>
		/// Nome da tabela como no SQL (ex: Orders).
		/// </summary>
		public string TableName { get; set; } = "";

		/// <summary>
		/// Colunas da tabela.
		/// </summary>
		public List<ParsedColumn> Columns { get; } = [];

		/// <summary>
		/// Chaves estrangeiras definidas na tabela.
		/// </summary>
		public List<ParsedForeignKey> ForeignKeys { get; } = [];
	}

	/// <summary>
	/// Coluna parseada ([Name] type NULL/NOT NULL).
	/// </summary>
	public sealed class ParsedColumn
	{
		/// <summary>
		/// Nome da coluna.
		/// </summary>
		public string Name { get; set; } = "";

		/// <summary>
		/// Tipo SQL bruto (ex: nvarchar(100), decimal(18,2), uniqueidentifier).
		/// </summary>
		public string SqlType { get; set; } = "";

		/// <summary>
		/// Se a coluna é NOT NULL.
		/// </summary>
		public bool IsRequired { get; set; }

		/// <summary>
		/// Comprimento para tipos string (nvarchar(n)).
		/// </summary>
		public int? Length { get; set; }

		/// <summary>
		/// Precisão para decimal.
		/// </summary>
		public int? Precision { get; set; }

		/// <summary>
		/// Escala para decimal.
		/// </summary>
		public int? Scale { get; set; }

		/// <summary>
		/// Indica se é coluna de chave primária (Id).
		/// </summary>
		public bool IsPrimaryKey { get; set; }
	}

	/// <summary>
	/// Chave estrangeira parseada (FOREIGN KEY ... REFERENCES).
	/// </summary>
	public sealed class ParsedForeignKey
	{
		/// <summary>
		/// Nome da coluna na tabela atual (ex: CustomerId).
		/// </summary>
		public string ColumnName { get; set; } = "";

		/// <summary>
		/// Schema da tabela referenciada (ex: dbo). Null quando não especificado (mesmo schema da origem).
		/// </summary>
		public string? ReferencedSchema { get; set; }

		/// <summary>
		/// Nome da tabela referenciada (ex: Customers).
		/// </summary>
		public string ReferencedTableName { get; set; } = "";

		/// <summary>
		/// Nome da coluna referenciada (geralmente Id).
		/// </summary>
		public string ReferencedColumnName { get; set; } = "";
	}
}
