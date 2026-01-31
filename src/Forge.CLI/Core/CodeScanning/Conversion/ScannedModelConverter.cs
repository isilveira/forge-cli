using Forge.CLI.Core.CodeScanning.Aggregation;
using Forge.CLI.Core.CodeScanning.Markers;
using Forge.CLI.Models;

namespace Forge.CLI.Core.CodeScanning.Conversion
{
	/// <summary>
	/// Converte um ScannedProjectModel (resultado do scan de marcações)
	/// para um ForgeProject (modelo interno do sistema).
	/// 
	/// Responsabilidade única: transformação de dados escaneados para o modelo de domínio.
	/// Não faz merge com projetos existentes - isso é responsabilidade do ProjectMerger.
	/// </summary>
	public sealed class ScannedModelConverter
	{
		/// <summary>
		/// Converte o modelo escaneado para um ForgeProject.
		/// Cria a estrutura hierárquica: Project → Contexts → Entities → Properties/Relations
		/// </summary>
		/// <param name="scanned">Modelo com markers agregados do scan</param>
		/// <param name="projectName">Nome do projeto (usado se não houver projeto existente)</param>
		/// <returns>ForgeProject construído a partir das marcações</returns>
		public ForgeProject Convert(ScannedProjectModel scanned, string projectName = "Scanned")
		{
			var project = new ForgeProject
			{
				Name = projectName,
				Contexts = new Dictionary<string, ForgeContext>()
			};

			// Passo 1: Criar entidades a partir dos markers de entidade
			// Isso estabelece a estrutura base de contextos e entidades
			foreach (var entityMarker in scanned.Entities)
			{
				EnsureEntityExists(project, entityMarker);
			}

			// Passo 2: Adicionar propriedades às entidades correspondentes
			// As propriedades referenciam entidades por nome
			foreach (var propertyMarker in scanned.Properties)
			{
				AddPropertyToEntity(project, propertyMarker);
			}

			// Passo 3: Adicionar relacionamentos às entidades correspondentes
			// Relacionamentos conectam entidades por nome
			foreach (var relationMarker in scanned.Relationships)
			{
				AddRelationToEntity(project, relationMarker);
			}

			return project;
		}

		/// <summary>
		/// Garante que uma entidade existe no projeto, criando contexto se necessário.
		/// </summary>
		private static void EnsureEntityExists(ForgeProject project, ForgeEntityMarker marker)
		{
			var contextName = string.IsNullOrWhiteSpace(marker.Context)
				? "Default"
				: marker.Context;

			// Criar contexto se não existir
			if (!project.Contexts.TryGetValue(contextName, out var context))
			{
				context = new ForgeContext
				{
					Description = contextName,
					Entities = new Dictionary<string, ForgeEntity>()
				};
				project.Contexts[contextName] = context;
			}

			// Criar entidade se não existir
			if (!context.Entities.ContainsKey(marker.Name))
			{
				context.Entities[marker.Name] = new ForgeEntity
				{
					Description = marker.Description,
					AggregateRoot = marker.AggregateRoot ?? true,
					Auditable = marker.Auditable ?? true,
					Properties = new Dictionary<string, ForgeProperty>(),
					Relations = new Dictionary<string, ForgeRelation>()
				};
			}
		}

		/// <summary>
		/// Adiciona uma propriedade à entidade correspondente.
		/// Se a entidade não existir, cria no contexto Default.
		/// </summary>
		private static void AddPropertyToEntity(ForgeProject project, ForgePropertyMarker marker)
		{
			// Encontrar a entidade pelo nome (busca em todos os contextos)
			var (context, entity) = FindOrCreateEntity(project, marker.Entity, marker.Context);

			if (entity is null)
				return; // Entidade não encontrada e não foi possível criar

			// Adicionar propriedade se não existir
			if (!entity.Properties.ContainsKey(marker.Name))
			{
				entity.Properties[marker.Name] = new ForgeProperty
				{
					Type = string.IsNullOrWhiteSpace(marker.Type) ? "string" : marker.Type,
					Required = marker.Required ?? false,
					Length = marker.Length,
					Precision = marker.Precision,
					Scale = marker.Scale
				};
			}
		}

		/// <summary>
		/// Adiciona um relacionamento à entidade de origem.
		/// </summary>
		private static void AddRelationToEntity(ForgeProject project, ForgeRelationshipMarker marker)
		{
			// Encontrar a entidade de origem
			var (context, entity) = FindOrCreateEntity(project, marker.From, marker.Context);

			if (entity is null)
				return;

			// Criar nome para o relacionamento baseado no destino
			var relationName = marker.To;

			if (!entity.Relations.ContainsKey(relationName))
			{
				entity.Relations[relationName] = new ForgeRelation
				{
					Type = NormalizeRelationKind(marker.Kind),
					Target = marker.To,
					Required = marker.Required ?? false
				};
			}
		}

		/// <summary>
		/// Encontra uma entidade pelo nome ou cria uma nova no contexto especificado.
		/// </summary>
		private static (ForgeContext? Context, ForgeEntity? Entity) FindOrCreateEntity(
			ForgeProject project,
			string entityName,
			string? contextHint)
		{
			// Primeiro, tentar encontrar em todos os contextos
			foreach (var (ctxName, ctx) in project.Contexts)
			{
				if (ctx.Entities.TryGetValue(entityName, out var found))
					return (ctx, found);
			}

			// Se não encontrou, criar no contexto especificado ou Default
			var targetContextName = string.IsNullOrWhiteSpace(contextHint)
				? "Default"
				: contextHint;

			if (!project.Contexts.TryGetValue(targetContextName, out var targetContext))
			{
				targetContext = new ForgeContext
				{
					Description = targetContextName,
					Entities = new Dictionary<string, ForgeEntity>()
				};
				project.Contexts[targetContextName] = targetContext;
			}

			// Criar entidade implícita (referenciada mas não declarada)
			var newEntity = new ForgeEntity
			{
				Description = null,
				AggregateRoot = true,
				Auditable = true,
				Properties = new Dictionary<string, ForgeProperty>(),
				Relations = new Dictionary<string, ForgeRelation>()
			};

			targetContext.Entities[entityName] = newEntity;

			return (targetContext, newEntity);
		}

		/// <summary>
		/// Normaliza o tipo de relacionamento para o formato esperado pelo modelo.
		/// </summary>
		private static string NormalizeRelationKind(string kind)
		{
			return kind.ToLowerInvariant() switch
			{
				"one-to-many" or "onetomany" or "1:n" => "one-to-many",
				"many-to-one" or "manytoone" or "n:1" => "many-to-one",
				"many-to-many" or "manytomany" or "n:n" => "many-to-many",
				"one-to-one" or "onetoone" or "1:1" => "one-to-one",
				_ => kind // Manter original se não reconhecido
			};
		}
	}
}
