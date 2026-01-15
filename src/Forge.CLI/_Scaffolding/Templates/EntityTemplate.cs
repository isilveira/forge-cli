using BAYSOFT.Abstractions.Crosscutting.Extensions;
using BAYSOFT.Abstractions.Crosscutting.Pluralization;
using BAYSOFT.Abstractions.Crosscutting.Pluralization.English;
using Forge.CLI.Models;
using Forge.CLI.Shared.Helpers;

namespace Forge.CLI.Scaffolding.Templates
{
    public class EntityTemplate
    {
		public static ForgeFile Render(ForgeProject project, string contextName, ForgeContext context, string entityName, ForgeEntity entity)
		{
			var entityCollectionName = Pluralizer.GetInstance()
				.AddEnglishPluralizer()
				.Pluralize(entityName, "en-US")
				.ToPascalCase();

			var filePath = $"src/{project.Name}.Core.Domain/{contextName}/Aggregates/{entityCollectionName}/Entities";
			var fileName = $"{entityName}.cs";

			var references = new List<string>()
			{
				$"BAYSOFT.Abstractions.Core.Domain.Entities",
				$"BAYSOFT.Abstractions.Crosscutting.InheritStringLocalization",
				$"{project.Name}.Core.Domain.{contextName}.Aggregates.{entityCollectionName}.Resources",
				$"{project.Name}.Core.Domain.{contextName}.Resources",
				$"{project.Name}.Core.Domain.Resources",
				$"System.Collections.Generic;"
			};
			entity.Relations.Values
				.Select(r => r.Target)
				.Distinct()
				.ToList()
				.ForEach(targetEntity =>
				{
					references.Add($"{project.Name}.Core.Domain.{contextName}.Aggregates.{Pluralizer.GetInstance()
						.AddEnglishPluralizer()
						.Pluralize(targetEntity, "en-US")
						.ToPascalCase()}.Entities");
				});

			var fileContent = $@"{RenderReferences(references)}
namespace {project.Name}.Core.Domain.{contextName}.Aggregates.{entityCollectionName}.Entities
{{
	[InheritStringLocalizer(typeof(Messages), Priority = 2)]
	[InheritStringLocalizer(typeof(Context{contextName}), Priority = 1)]
	[InheritStringLocalizer(typeof(Entities{entityCollectionName}), Priority = 0)]
	public class {entityName} : DomainEntity<{TypeMapperHelper.Map(project.IdType)}>
	{{
		#region Properties
{RenderProperties(entity)}
{RenderForeignKeys(project.IdType, entity)}
		#endregion

		#region Relations
{RenderRelations(entity)}
		#endregion

		#region Constructors
		public {entityName}()
		{{
		}}
		#endregion
	}}
}}";

			return new ForgeFile(filePath, fileName, fileContent);
		}

		private static string RenderReferences(List<string> references)
		{
			var result = string.Empty;
			foreach (var reference in references.Order())
			{
				result += $"using {reference};{Environment.NewLine}";
			}
			return result;
		}
		private static string RenderProperties(ForgeEntity entity)
		{
			var result = string.Empty;
			foreach (var property in entity.Properties)
			{
				result += $"\t\tpublic {TypeMapperHelper.Map(property.Value.Type)}{(property.Value.Required ? "": "?")} {property.Key} {{ get; set; }}{Environment.NewLine}";
			}
			return result;
		}
		private static string RenderForeignKeys(string projectIdType, ForgeEntity entity)
		{
			var result = string.Empty;
			foreach (var relation in entity.Relations)
			{
				if (relation.Value.Type == "many-to-one")
				{
					result += $"\t\tpublic {TypeMapperHelper.Map(projectIdType)}{(relation.Value.Required ? "": "?")} {relation.Key}Id {{ get; set; }}{Environment.NewLine}";
				}
			}
			return result;
		}
		private static string RenderRelations(ForgeEntity entity)
		{
			var result = string.Empty;
			foreach (var relation in entity.Relations)
			{
				if (relation.Value.Type == "many-to-one")
				{
					result += $"\t\tpublic {relation.Value.Target}{(relation.Value.Required ? "":"?")} {relation.Key} {{ get; set; }}{Environment.NewLine}";
				}
				else
				{
					result += $"\t\tpublic List<{relation.Value.Target}> {relation.Key} {{ get; set; }} = new();{Environment.NewLine}";
				}
			}
			return result;
		}
	}
}
