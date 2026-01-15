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

		public IReadOnlyCollection<ScaffoldTarget> Resolve(
			string? entityName,
			string? contextName)
		{
			var targets = new List<ScaffoldTarget>();

			// Projeto inteiro
			if (entityName is null && contextName is null)
			{
				foreach (var keyContext in _project.Contexts)
				{
					foreach (var keyEntity in keyContext.Value.Entities)
					{
						targets.Add(new ScaffoldTarget
						{
							Scope = TargetScope.Entity,
							ContextName = keyContext.Key,
							EntityName = keyEntity.Key
						});
					}
				}

				return targets;
			}

			// Apenas contexto
			if (entityName is null || entityName == "all")
			{
				var context = GetContext(contextName);

				foreach (var entity in context.Entities.Values)
				{
					targets.Add(new ScaffoldTarget
					{
						Scope = TargetScope.Entity,
						ContextName = contextName,
						EntityName = entityName
					});
				}

				return targets;
			}

			// Entidade + contexto
			if (contextName is not null)
			{
				var context = GetContext(contextName);
				var entity = GetEntity(contextName, context, entityName);

				targets.Add(new ScaffoldTarget
				{
					Scope = TargetScope.Entity,
					ContextName = contextName,
					EntityName = entityName
				});

				return targets;
			}

			throw new InvalidOperationException(
				"Entity specified without context.");
		}
		private ForgeContext GetContext(string? name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new InvalidOperationException("Context not specified.");

			if (!_project.Contexts.TryGetValue(name, out var context))
				throw new InvalidOperationException($"Context '{name}' not found.");

			return context;
		}
		private ForgeEntity GetEntity(
			string contextName,
			ForgeContext context,
			string name)
		{
			if (!context.Entities.TryGetValue(name, out var entity))
				throw new InvalidOperationException(
					$"Entity '{name}' not found in context '{contextName}'.");

			return entity;
		}
	}
}
