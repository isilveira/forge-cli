using Forge.CLI.Models;
using Forge.CLI.Scaffolding.Templates;

namespace Forge.CLI.Scaffolding.Generators
{
	public sealed class EntityResourceScaffoldGenerator
	{
		public List<ForgeFile> Generate(ForgeProject project, string contextName, ForgeContext context, string entityName, ForgeEntity entity)
		{
			return new List<ForgeFile> 
			{
				EntityResourceDesignerTemplate.Render(project, contextName, context, entityName, entity),
				EntityResourceTemplate.Render(project, contextName, context, entityName, entity),
				EntityResourceTemplate.Render(project, contextName, context, entityName, entity, "pt-BR")
			};
		}
	}
}