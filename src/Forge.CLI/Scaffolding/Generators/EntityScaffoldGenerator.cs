using Forge.CLI.Models;
using Forge.CLI.Scaffolding.Templates;

namespace Forge.CLI.Scaffolding.Generators
{
	public sealed class EntityScaffoldGenerator
	{
		public List<ForgeFile> Generate(ForgeProject project, string contextName, ForgeContext context, string entityName, ForgeEntity entity)
		{
			return new List<ForgeFile> 
			{
				EntityTemplate.Render(project, contextName, context, entityName, entity)
			};
		}
	}
}