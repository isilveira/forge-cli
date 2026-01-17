using Forge.CLI.Core.Planning;
using Forge.CLI.Models;
using System;

namespace Forge.CLI.Core.Target
{
	public sealed class TargetResolver
	{
		private readonly ForgeProject _project;

		public TargetResolver(ForgeProject project)
		{
			_project = project;
		}

		public IReadOnlyCollection<ScaffoldTarget> Resolve(
			ScaffoldRequest request)
		{
			var targets = new List<ScaffoldTarget>();

			if(string.IsNullOrWhiteSpace(request.ContextName)
				&& string.IsNullOrWhiteSpace(request.EntityName))
			{
				targets.Add(new ScaffoldTarget
				{
					Project = _project,
					Scope = TargetScope.Project,
				});
			}

			var contexts = _project.Contexts
				.Where(c =>
					string.IsNullOrWhiteSpace(request.ContextName)
					|| c.Key.Equals(request.ContextName)
				)
				.ToList();

			foreach (var context in contexts)
			{
				targets.Add(new ScaffoldTarget
				{
					Project = _project,
					Scope = TargetScope.Context,
					ContextName = context.Key,
				});

				var entities = context.Value.Entities
					.Where(e =>
						string.IsNullOrWhiteSpace(request.EntityName)
						|| e.Key.Equals(request.EntityName)
					)
					.ToList();

				foreach (var entity in entities)
				{
					targets.Add(new ScaffoldTarget
					{
						Project = _project,
						Scope = TargetScope.Entity,
						ContextName = context.Key,
						EntityName = entity.Key,
						Name = request.Name
					});
				}
			}

			return targets;
		}
	}
}
