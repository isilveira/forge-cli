using Forge.CLI.Core.Planning;
using Forge.CLI.Models;

namespace Forge.CLI.Core.Target
{
	public sealed class TargetResolver
	{
		private readonly ForgeProject _project;

		public TargetResolver(ForgeProject project)
		{
			_project = project;
		}

		public ScaffoldTarget Resolve(
			ScaffoldRequest request)
		{
			var targets = new List<ScaffoldTarget>();

			if (request.All)
			{
				return new ScaffoldTarget
				{
					Scope = TargetScope.Project,
					AllContexts = true,
					AllEntities = true
				};
			}

			if(!string.IsNullOrWhiteSpace(request.ContextName)
				&& !string.IsNullOrWhiteSpace(request.EntityName))
			{
				return new ScaffoldTarget
				{
					Scope = TargetScope.Entity,
					ContextName = request.ContextName,
					EntityName = request.EntityName
				};
			}

			if(!string.IsNullOrWhiteSpace(request.ContextName))
			{
				return new ScaffoldTarget
				{
					Scope = TargetScope.Context,
					ContextName = request.ContextName,
					AllEntities = true
				};
			}

			throw new InvalidOperationException(
				"Informe --context ou --all.");
		}
	}
}
