using Forge.CLI.Models;
using Spectre.Console;

namespace Forge.CLI.Core.CodeScanning.Merging
{
	/// <summary>
	/// Realiza o merge entre um projeto base (existente) e um projeto escaneado.
	/// 
	/// Responsabilidades:
	/// - Combinar dados de dois ForgeProject
	/// - Respeitar configurações de overwrite via MergeOptions
	/// - Rastrear alterações via MergeResult
	/// 
	/// Não é responsável por:
	/// - Carregar/salvar projetos (use ProjectLoader/ProjectSaver)
	/// - Converter ScannedProjectModel (use ScannedModelConverter)
	/// </summary>
	public sealed class ProjectMerger
	{
		/// <summary>
		/// Faz merge do projeto escaneado no projeto base.
		/// O projeto base é modificado in-place.
		/// </summary>
		/// <param name="baseProject">Projeto existente que será atualizado</param>
		/// <param name="scannedProject">Projeto construído a partir das marcações</param>
		/// <param name="options">Opções de controle do merge</param>
		/// <returns>Resultado com estatísticas do merge</returns>
		public MergeResult Merge(
			ForgeProject baseProject,
			ForgeProject scannedProject,
			MergeOptions options)
		{
			var result = new MergeResult();

			// Iterar sobre todos os contextos do projeto escaneado
			foreach (var (contextName, scannedContext) in scannedProject.Contexts)
			{
				MergeContext(baseProject, contextName, scannedContext, options, result);
			}

			return result;
		}

		/// <summary>
		/// Faz merge de um contexto específico.
		/// </summary>
		private static void MergeContext(
			ForgeProject baseProject,
			string contextName,
			ForgeContext scannedContext,
			MergeOptions options,
			MergeResult result)
		{
			// Verificar se contexto existe no projeto base
			if (!baseProject.Contexts.TryGetValue(contextName, out var baseContext))
			{
				if (!options.CreateMissingContexts)
				{
					result.Skipped++;
					result.Warnings.Add($"Context '{contextName}' not found and CreateMissingContexts=false");
					return;
				}

				// Criar novo contexto
				baseContext = new ForgeContext
				{
					Description = scannedContext.Description,
					Entities = new Dictionary<string, ForgeEntity>()
				};
				baseProject.Contexts[contextName] = baseContext;
				result.ContextsCreated++;
			}

			// Merge das entidades do contexto
			foreach (var (entityName, scannedEntity) in scannedContext.Entities)
			{
				MergeEntity(baseContext, entityName, scannedEntity, options, result);
			}
		}

		/// <summary>
		/// Faz merge de uma entidade específica.
		/// </summary>
		private static void MergeEntity(
			ForgeContext baseContext,
			string entityName,
			ForgeEntity scannedEntity,
			MergeOptions options,
			MergeResult result)
		{
			if (!baseContext.Entities.TryGetValue(entityName, out var baseEntity))
			{
				if (!options.CreateMissingEntities)
				{
					result.Skipped++;
					result.Warnings.Add($"Entity '{entityName}' not found and CreateMissingEntities=false");
					return;
				}

				// Criar nova entidade
				baseEntity = new ForgeEntity
				{
					IdType = scannedEntity.IdType,
					Table = scannedEntity.Table,
					Description = scannedEntity.Description,
					AggregateRoot = scannedEntity.AggregateRoot,
					Auditable = scannedEntity.Auditable,
					Properties = new Dictionary<string, ForgeProperty>(),
					Relations = new Dictionary<string, ForgeRelation>()
				};
				baseContext.Entities[entityName] = baseEntity;
				result.EntitiesCreated++;
			}
			else if (options.OverwriteEntities)
			{
				// Atualizar metadados da entidade (mas preservar properties e relations)

				baseEntity.IdType = scannedEntity.IdType;
				baseEntity.Table = scannedEntity.Table;
				baseEntity.Description = scannedEntity.Description ?? baseEntity.Description;
				baseEntity.AggregateRoot = scannedEntity.AggregateRoot;
				baseEntity.Auditable = scannedEntity.Auditable;
				result.EntitiesUpdated++;
			}

			// Merge das propriedades
			foreach (var (propName, scannedProp) in scannedEntity.Properties)
			{
				MergeProperty(baseEntity, propName, scannedProp, options, result);
			}

			// Merge dos relacionamentos
			foreach (var (relName, scannedRel) in scannedEntity.Relations)
			{
				MergeRelation(baseEntity, relName, scannedRel, options, result);
			}
		}

		/// <summary>
		/// Faz merge de uma propriedade específica.
		/// </summary>
		private static void MergeProperty(
			ForgeEntity baseEntity,
			string propName,
			ForgeProperty scannedProp,
			MergeOptions options,
			MergeResult result)
		{
			if (!baseEntity.Properties.TryGetValue(propName, out var baseProp))
			{
				if (!options.CreateMissingProperties)
				{
					result.Skipped++;
					return;
				}

				// Criar nova propriedade
				baseEntity.Properties[propName] = new ForgeProperty
				{
					Type = scannedProp.Type,
					Required = scannedProp.Required,
					Length = scannedProp.Length,
					HasMaxLength = scannedProp.HasMaxLength,
					Precision = scannedProp.Precision,
					Scale = scannedProp.Scale
				};
				result.PropertiesCreated++;
			}
			else if (options.OverwriteProperties)
			{
				// Atualizar propriedade existente
				baseProp.Type = scannedProp.Type;
				baseProp.Required = scannedProp.Required;
				baseProp.Length = scannedProp.Length;
				baseProp.HasMaxLength = scannedProp.HasMaxLength;
				baseProp.Precision = scannedProp.Precision;
				baseProp.Scale = scannedProp.Scale;
				result.PropertiesUpdated++;
			}
			else
			{
				result.Skipped++;
			}
		}

		/// <summary>
		/// Faz merge de um relacionamento específico.
		/// </summary>
		private static void MergeRelation(
			ForgeEntity baseEntity,
			string relName,
			ForgeRelation scannedRel,
			MergeOptions options,
			MergeResult result)
		{
			if (!baseEntity.Relations.TryGetValue(relName, out var baseRel))
			{
				if (!options.CreateMissingRelations)
				{
					result.Skipped++;
					return;
				}

				// Criar novo relacionamento
				baseEntity.Relations[relName] = new ForgeRelation
				{
					Type = scannedRel.Type,
					Target = scannedRel.Target,
					Required = scannedRel.Required
				};
				result.RelationsCreated++;
			}
			else if (options.OverwriteRelations)
			{
				// Atualizar relacionamento existente
				baseRel.Type = scannedRel.Type;
				baseRel.Target = scannedRel.Target;
				baseRel.Required = scannedRel.Required;
				result.RelationsUpdated++;
			}
			else
			{
				result.Skipped++;
			}
		}
	}
}
