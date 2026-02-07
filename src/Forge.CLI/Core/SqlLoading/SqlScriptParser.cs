using System;
using System.Text.RegularExpressions;

namespace Forge.CLI.Core.SqlLoading
{
	/// <summary>
	/// Parser de scripts SQL de criação de tabelas no estilo das migrations do Entity Framework.
	/// Suporta formato SQL Server: CREATE TABLE [schema].[TableName] ( ... CONSTRAINT ... ).
	/// </summary>
	public sealed class SqlScriptParser
	{
		// CREATE TABLE [schema].[TableName] ou CREATE TABLE TableName
		private static readonly Regex CreateTableRegex = new(
			@"CREATE\s+TABLE\s+(?:\[?(\w+)\]?\.)?\[?(\w+)\]?\s*\(",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// [ColumnName] type [(params)] NULL/NOT NULL
		private static readonly Regex ColumnRegex = new(
			@"\[(\w+)\]\s+(\w+)(?:\((\d+)\)|\((?:max)\)|\((\d+),\s*(\d+)\))?\s+(NULL|NOT\s+NULL)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// CONSTRAINT [PK_xxx] PRIMARY KEY ([Id])
		private static readonly Regex PrimaryKeyRegex = new(
			@"CONSTRAINT\s+\[?\w+\]?\s+PRIMARY\s+KEY\s+\(\[?(\w+)\]?\)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// CONSTRAINT [FK_xxx] FOREIGN KEY ([Col]) REFERENCES [schema].[Table] ([RefCol])
		private static readonly Regex ForeignKeyRegex = new(
			@"CONSTRAINT\s+\[?\w+\]?\s+FOREIGN\s+KEY\s+\(\[?(\w+)\]?\)\s+REFERENCES\s+(?:\[?(\w+)\]?\.)?\[?(\w+)\]?\s+\(\[?(\w+)\]?\)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// Faz o parsing do conteúdo do script SQL e retorna o modelo parseado.
		/// </summary>
		/// <param name="sqlContent">Conteúdo do script SQL (texto completo).</param>
		/// <returns>Modelo com tabelas, colunas e FKs extraídas.</returns>
		public ParsedSqlModel Parse(string sqlContent)
		{
			var model = new ParsedSqlModel();

			// Normalizar: remover comentários de linha e blocos
			var normalized = RemoveSqlComments(sqlContent);

			// Encontrar cada bloco CREATE TABLE ... ( ... );
			var tableBlocks = ExtractCreateTableBlocks(normalized);

			foreach (var block in tableBlocks)
			{
				var table = ParseTableBlock(block);
				if (table != null)
					model.Tables.Add(table);
			}

			return model;
		}

		/// <summary>
		/// Remove comentários SQL (-- e /* */).
		/// </summary>
		private static string RemoveSqlComments(string sql)
		{
			// Remover /* ... */
			sql = Regex.Replace(sql, @"/\*[\s\S]*?\*/", " ", RegexOptions.Multiline);
			// Remover -- até fim da linha
			sql = Regex.Replace(sql, @"--[^\r\n]*", " ", RegexOptions.Multiline);
			return sql;
		}

		/// <summary>
		/// Extrai blocos de CREATE TABLE ( ... ); do script.
		/// </summary>
		private static List<string> ExtractCreateTableBlocks(string sql)
		{
			var blocks = new List<string>();
			var match = CreateTableRegex.Match(sql);

			while (match.Success)
			{
				var start = match.Index;
				var parenStart = sql.IndexOf("(", start, StringComparison.Ordinal);
				if (parenStart < 0) break;

				var depth = 1;
				var i = parenStart + 1;
				while (i < sql.Length && depth > 0)
				{
					var c = sql[i];
					if (c == '(') depth++;
					else if (c == ')') depth--;
					i++;
				}

				var end = i;
				var block = sql[start..end];
				blocks.Add(block);
				match = CreateTableRegex.Match(sql, end);
			}

			return blocks;
		}

		/// <summary>
		/// Parseia um bloco CREATE TABLE ( ... ).
		/// </summary>
		private static ParsedTable? ParseTableBlock(string block)
		{
			var tableMatch = CreateTableRegex.Match(block);
			if (!tableMatch.Success)
				return null;

			var schema = tableMatch.Groups[1].Success && !string.IsNullOrEmpty(tableMatch.Groups[1].Value)
				? tableMatch.Groups[1].Value
				: null;
			var tableName = tableMatch.Groups[2].Value;

			var table = new ParsedTable
			{
				Schema = schema,
				TableName = tableName
			};

			// Colunas: [Name] type NULL/NOT NULL (ignorar linhas que são CONSTRAINT)
			var columnMatches = ColumnRegex.Matches(block);
			var pkColumnName = (string?)null;

			foreach (Match colMatch in columnMatches)
			{
				var colName = colMatch.Groups[1].Value;
				var sqlType = colMatch.Groups[2].Value;
				var isRequired = colMatch.Groups[6].Value.Trim().StartsWith("NOT", StringComparison.OrdinalIgnoreCase);

				int? length = null;
				int? precision = null;
				int? scale = null;

				if (colMatch.Groups[3].Success && !string.IsNullOrEmpty(colMatch.Groups[3].Value))
					length = int.Parse(colMatch.Groups[3].Value);
				if (colMatch.Groups[4].Success && !string.IsNullOrEmpty(colMatch.Groups[4].Value))
					precision = int.Parse(colMatch.Groups[4].Value);
				if (colMatch.Groups[5].Success && !string.IsNullOrEmpty(colMatch.Groups[5].Value))
					scale = int.Parse(colMatch.Groups[5].Value);

				// nvarchar(max) / varchar(max) não tem número no regex acima; tratar tipo "max"
				if (sqlType.Equals("nvarchar", StringComparison.OrdinalIgnoreCase) ||
					sqlType.Equals("varchar", StringComparison.OrdinalIgnoreCase))
				{
					var maxMatch = Regex.Match(block, $@"\[{colName}\]\s+{sqlType}\s*\(\s*max\s*\)", RegexOptions.IgnoreCase);
					if (maxMatch.Success)
						length = null; // max = sem length
				}

				table.Columns.Add(new ParsedColumn
				{
					Name = colName,
					SqlType = sqlType,
					IsRequired = isRequired,
					Length = length,
					Precision = precision,
					Scale = scale
				});
			}

			// PRIMARY KEY
			var pkMatch = PrimaryKeyRegex.Match(block);
			if (pkMatch.Success)
			{
				pkColumnName = pkMatch.Groups[1].Value;
				var pkCol = table.Columns.FirstOrDefault(c => c.Name.Equals(pkColumnName, StringComparison.OrdinalIgnoreCase));
				if (pkCol != null)
					pkCol.IsPrimaryKey = true;
			}

			// FOREIGN KEY: REFERENCES [schema].[Table] ([RefCol]) -> groups 2=schema, 3=table, 4=refcol
			var fkMatches = ForeignKeyRegex.Matches(block);
			foreach (Match fkMatch in fkMatches)
			{
				var refSchema = fkMatch.Groups[2].Success && !string.IsNullOrEmpty(fkMatch.Groups[2].Value)
					? fkMatch.Groups[2].Value
					: null;

				table.ForeignKeys.Add(new ParsedForeignKey
				{
					ColumnName = fkMatch.Groups[1].Value,
					ReferencedSchema = refSchema,
					ReferencedTableName = fkMatch.Groups[3].Value,
					ReferencedColumnName = fkMatch.Groups[4].Value
				});
			}

			return table;
		}
	}
}
