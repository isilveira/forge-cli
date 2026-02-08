using BAYSOFT.Abstractions.Crosscutting.Extensions;
using Forge.CLI.Models;
using Forge.CLI.Shared.Extensions;
using Forge.CLI.Shared.Helpers;
using YamlDotNet.Serialization;

namespace Forge.CLI.Core.SqlLoading
{
	/// <summary>
	/// Converte o modelo parseado de um script SQL (ParsedSqlModel) em ForgeProject.
	/// Schema vira contexto; tabelas viram entidades (nome singularizado); colunas viram propriedades; FKs viram relações many-to-one.
	/// </summary>
	public sealed class SqlToForgeProjectConverter
	{
		/// <summary>
		/// Nome do contexto quando o schema não é especificado no SQL.
		/// </summary>
		public const string DefaultContextName = "Default";

		/// <summary>
		/// Sufixos removidos da coluna FK para obter o nome da relação (case-insensitive).
		/// Ordem: mais longa primeiro para evitar remoção parcial (ex: _id antes de id).
		/// </summary>
		private static readonly string[] FkColumnSuffixesToRemove = ["_id", "_key", "id", "key"];

		/// <summary>
		/// Converte o modelo SQL parseado em ForgeProject.
		/// Múltiplos contextos: um por schema (dbo, Sales, etc.). Schema null vira contexto "Default".
		/// </summary>
		/// <param name="parsed">Modelo extraído do script SQL.</param>
		/// <param name="projectName">Nome do projeto.</param>
		/// <param name="singleContextName">Se informado, ignora schema e coloca todas as tabelas neste contexto (comportamento legado).</param>
		/// <returns>ForgeProject populado.</returns>
		public ForgeProject Convert(
			ParsedSqlModel parsed,
			string projectName = "Project",
			string? singleContextName = null)
		{
			var project = new ForgeProject
			{
				Name = projectName,
				Contexts = new Dictionary<string, ForgeContext>()
			};

			// Agrupar tabelas por schema (schema = contexto)
			var tablesBySchema = parsed.Tables
				.GroupBy(t => string.IsNullOrWhiteSpace(t.Schema) ? DefaultContextName : t.Schema!)
				.ToDictionary(g => g.Key, g => g.ToList());

			// Se singleContextName foi informado, colocar todas as tabelas em um único contexto
			if (!string.IsNullOrWhiteSpace(singleContextName))
			{
				var allTables = parsed.Tables.ToList();
				tablesBySchema = new Dictionary<string, List<ParsedTable>> { [singleContextName] = allTables };
			}
			var convertSchemaToContextName = (string schema) => schema.EndsWith("Db") ? schema.Replace("Db", "") : schema;
			// Criar um contexto por schema
			foreach (var (schemaName, tables) in tablesBySchema)
			{
				var context = new ForgeContext
				{
					Description = convertSchemaToContextName(schemaName),
					Entities = new Dictionary<string, ForgeEntity>()
				};
				project.Contexts[convertSchemaToContextName(schemaName)] = context;

				// Criar entidades (singularizar nome da tabela)
				foreach (var table in tables)
				{
					var entityName = table.TableName.SingularizeAsPascal();
					if (string.IsNullOrEmpty(entityName))
						entityName = table.TableName;

					if (!context.Entities.ContainsKey(entityName))
					{
						context.Entities[entityName] = new ForgeEntity
						{
							Properties = new Dictionary<string, ForgeProperty>(),
							Relations = new Dictionary<string, ForgeRelation>()
						};
					}

					var primaryKeys = table.Columns.Where(c => c.IsPrimaryKey).ToList();
					if (primaryKeys.Count == 1)
					{
						var pkColumn = primaryKeys[0];
						context.Entities[entityName].IdType = TypeMapperHelper.Map(SqlTypeMapper.ToForgeType(pkColumn.SqlType));
					}
					if (table.ForeignKeys.Any())
					{
						context.Entities[entityName].AggregateRoot = false;
					}
				}
			}

			// Preencher propriedades e relações por contexto
			foreach (var (schemaName, tables) in tablesBySchema)
			{
				var context = project.Contexts[convertSchemaToContextName(schemaName)];

				foreach (var table in tables)
				{
					var entityName = table.TableName.SingularizeAsPascal();
					if (string.IsNullOrEmpty(entityName))
						entityName = table.TableName;

					var entity = context.Entities[entityName];
					var fkColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					foreach (var fk in table.ForeignKeys)
						fkColumns.Add(fk.ColumnName);

					// Propriedades (exceto Id e colunas que são FK)
					foreach (var col in table.Columns)
					{
						if (col.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && col.IsPrimaryKey)
							continue;
						if (fkColumns.Contains(col.Name))
							continue;

						var forgeType = TypeMapperHelper.Map(SqlTypeMapper.ToForgeType(col.SqlType, col.Length, col.Precision, col.Scale));

						entity.Properties[col.Name] = new ForgeProperty
						{
							Type = forgeType,
							Required = col.IsRequired,
							Length = SqlTypeMapper.HasLength(forgeType) ? col.Length : null,
							HasMaxLength = SqlTypeMapper.HasLength(forgeType) && col.HasMaxLength.HasValue && col.HasMaxLength.Value,
							Precision = SqlTypeMapper.HasPrecision(forgeType) ? col.Precision : null,
							Scale = SqlTypeMapper.HasScale(forgeType) ? col.Scale : null
						};
					}

					// Relações: nome derivado da coluna FK (removendo id, _id, key, _key)
					foreach (var fk in table.ForeignKeys)
					{
						// Com contexto único forçado, o alvo está no mesmo contexto; senão usa schema referenciado ou da tabela
						var targetSchema = !string.IsNullOrWhiteSpace(singleContextName)
							? singleContextName
							: (!string.IsNullOrWhiteSpace(fk.ReferencedSchema)
								? convertSchemaToContextName(fk.ReferencedSchema)
								: (string.IsNullOrWhiteSpace(table.Schema) ? DefaultContextName : convertSchemaToContextName(table.Schema)));

						// Só adiciona relação se o contexto alvo existir (pode ser outro schema)
						if (!project.Contexts.TryGetValue(targetSchema!, out var targetContext))
							continue;

						var targetEntityName = fk.ReferencedTableName.SingularizeAsPascal();
						if (string.IsNullOrEmpty(targetEntityName))
							targetEntityName = fk.ReferencedTableName;

						if (!targetContext.Entities.ContainsKey(targetEntityName))
							continue;

						var relationName = DeriveRelationNameFromFkColumn(fk.ColumnName);
						if (string.IsNullOrEmpty(relationName))
							relationName = targetEntityName;

						var fkColumnRequired = table.Columns
							.FirstOrDefault(c => c.Name.Equals(fk.ColumnName, StringComparison.OrdinalIgnoreCase))?.IsRequired ?? false;

						if (!entity.Relations.ContainsKey(relationName))
						{
							entity.Relations[relationName] = new ForgeRelation
							{
								Type = "many-to-one",
								Target = targetEntityName,
								Required = fkColumnRequired
							};
						}
						var entityCollectionName = entityName.PluralizeAsPascal();
						var targetEntity = targetContext.Entities[targetEntityName];
						var targetRelationName = !string.IsNullOrWhiteSpace(relationName) && !relationName.ToLower().Equals(targetEntityName.ToLower())
							? $"{relationName}{entityCollectionName}"
							: $"{entityCollectionName}";

						if (!targetEntity.Relations.ContainsKey(targetRelationName))
						{
							targetEntity.Relations[$"{targetRelationName}"] = new ForgeRelation
							{
								Type = "one-to-many",
								Target = entityName,
								Required = fkColumnRequired
							};
						}
					}
				}
			}

			return project;
		}

		/// <summary>
		/// Deriva o nome da relação a partir do nome da coluna FK,
		/// removendo sufixos comuns: id, _id, key, _key (case-insensitive) e convertendo para PascalCase.
		/// </summary>
		/// <example>CustomerId -> Customer; customer_id -> Customer; ParentKey -> Parent; parent_key -> Parent</example>
		public static string DeriveRelationNameFromFkColumn(string columnName)
		{
			if (string.IsNullOrWhiteSpace(columnName))
				return columnName;

			var s = columnName.Trim();

			foreach (var suffix in FkColumnSuffixesToRemove)
			{
				if (s.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) && s.Length > suffix.Length)
				{
					s = s[..^suffix.Length];
					break; // remove apenas um sufixo
				}
			}

			return s.ToPascalCase();
		}
	}
}
