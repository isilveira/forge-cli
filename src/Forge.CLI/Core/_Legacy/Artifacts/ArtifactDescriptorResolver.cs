using BAYSOFT.Abstractions.Crosscutting.Pluralization;
using BAYSOFT.Abstractions.Crosscutting.Pluralization.English;
using Forge.CLI.Core._Legacy.Capabilities;
using Forge.CLI.Core._Legacy.Planning;
using Forge.CLI.Core._Legacy.Target;
using Forge.CLI.Shared.Extensions;

namespace Forge.CLI.Core._Legacy.Artifacts
{
	public sealed class ArtifactDescriptorResolver
	{
		public ArtifactDescriptor Resolve(ScaffoldTask task)
		{
			var scope = ResolveScope(task.Type);

			var path = ResolvePath(task);
			var fileName = ResolveFileName(task);
			var templateKey = ResolveTemplateKey(task);

			return new ArtifactDescriptor
			{
				Layer = task.Layer,
				Type = task.Type,
				Variant = task.Variant,
				Scope = scope,

				RelativePath = path,
				FileName = fileName,
				TemplateKey = templateKey,

				Target = task.Target
			};
		}
		private static TargetScope ResolveScope(ArtifactType type)
		{
			return type switch
			{
				ArtifactType.Resource => TargetScope.Project,
				ArtifactType.DbContext => TargetScope.Context,
				ArtifactType.ContextResource => TargetScope.Context,
				_ => TargetScope.Entity
			};
		}
		private static string ResolvePath(
			ScaffoldTask task)
		{
			var context = task.Target.ContextName;
			var entity = task.Target.EntityName;
			var entityCollection = entity?.PluralizeAsPascal();

			switch (task.Layer)
			{
				case Layer.Application:
					{
						switch (task.Type)
						{
							case ArtifactType.Command:
								return $"src\\{task.Target.Project.Name}.Core.Application\\{context}\\Aggregates\\{entityCollection}\\Commands";
							case ArtifactType.Query:
								return $"src\\{task.Target.Project.Name}.Core.Application\\{context}\\Aggregates\\{entityCollection}\\Queries";
							case ArtifactType.Notification:
								return $"src\\{task.Target.Project.Name}.Core.Application\\{context}\\Aggregates\\{entityCollection}\\Notifications";
							default:
								throw new InvalidOperationException();
						}
					}
				case Layer.Domain:
					{
						switch (task.Type)
						{
							case ArtifactType.Entity:
								return $"src\\{task.Target.Project.Name}.Core.Domain\\{context}\\Aggregates\\{entityCollection}\\Entities";
							case ArtifactType.EntityResource:
								return $"src\\{task.Target.Project.Name}.Core.Domain\\{context}\\Aggregates\\{entityCollection}\\Resources";
							case ArtifactType.Service:
								return $"src\\{task.Target.Project.Name}.Core.Domain\\{context}\\Aggregates\\{entityCollection}\\Services";
							case ArtifactType.Validation:
								{
									switch (task.Variant)
									{
										case Variant.Entity:
											return $"src\\{task.Target.Project.Name}.Core.Domain\\{context}\\Aggregates\\{entityCollection}\\Validations\\EntityValidations";
										default:
											return $"src\\{task.Target.Project.Name}.Core.Domain\\{context}\\Aggregates\\{entityCollection}\\Validations\\DomainValidations";
									}
								}
							case ArtifactType.Specification:
								return $"src\\{task.Target.Project.Name}.Core.Domain\\{context}\\Aggregates\\{entityCollection}\\Specifications";
							case ArtifactType.ContextResource:
								return $"src\\{task.Target.Project.Name}.Core.Domain\\{context}\\Resources";
							case ArtifactType.IDbContextReader:
								return $"src\\{task.Target.Project.Name}.Core.Domain\\{context}\\Interfaces\\Infrastructures\\Data";
							case ArtifactType.IDbContextWriter:
								return $"src\\{task.Target.Project.Name}.Core.Domain\\{context}\\Interfaces\\Infrastructures\\Data";
							case ArtifactType.Resource:
								return $"src\\{task.Target.Project.Name}.Core.Domain\\Resources";
							case ArtifactType.IService:
								return $"src\\{task.Target.Project.Name}.Core.Domain\\Interfaces\\Infrastructures\\Services";
							default:
								throw new InvalidOperationException();
						}
					}
				case Layer.Infrastructure:
					{
						switch (task.Type)
						{
							case ArtifactType.Mapping:
								return $"src\\{task.Target.Project.Name}.Infrastructures.Data\\{context}\\EntityMappings";
							case ArtifactType.DbContext:
								return $"src\\{task.Target.Project.Name}.Infrastructures.Data\\{context}";
							case ArtifactType.DbContextReader:
								return $"src\\{task.Target.Project.Name}.Infrastructures.Data\\{context}";
							case ArtifactType.DbContextWriter:
								return $"src\\{task.Target.Project.Name}.Infrastructures.Data\\{context}";
							case ArtifactType.Service:
								return $"src\\{task.Target.Project.Name}.Infrastructures.Services";
							default:
								throw new InvalidOperationException();
						}
					}
				case Layer.Middleware:
					{
						switch (task.Type)
						{
							case ArtifactType.DbContextConfigurations:
								return $"src\\{task.Target.Project.Name}.Middleware\\AddServices";
							case ArtifactType.DomainServicesConfigurations:
								return $"src\\{task.Target.Project.Name}.Middleware\\AddServices";
							case ArtifactType.ValidationsConfigurations:
								return $"src\\{task.Target.Project.Name}.Middleware\\AddServices";
							case ArtifactType.Configurations:
								return $"src\\{task.Target.Project.Name}.Middleware";
							default:
								throw new InvalidOperationException();
						}
					}
				case Layer.Web:
					{
						switch (task.Type)
						{
							case ArtifactType.Api:
								{
									switch (task.Variant)
									{
										case Variant.Controller:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.Api\\Resources";
										default:
											throw new InvalidOperationException();
									}
								}
							case ArtifactType.React:
								{
									switch (task.Variant)
									{
										case Variant.Index:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.React\\client-app\\pages\\{entityCollection}";
										case Variant.Tab:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.React\\client-app\\organisms";
										case Variant.Form:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.React\\client-app\\organisms";
										case Variant.Table:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.React\\client-app\\organisms";
										case Variant.PageIndex:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.React\\client-app\\pages\\{entityCollection}";
										case Variant.PageCreate:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.React\\client-app\\pages\\{entityCollection}";
										case Variant.PageEdit:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.React\\client-app\\pages\\{entityCollection}";
										default:
											throw new InvalidOperationException();
									}
								}
							case ArtifactType.Blazor:
								{
									switch (task.Variant)
									{
										case Variant.Filter:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.Blazor\\Components\\Pages\\{context}\\{entityCollection}\\Helpers";
										case Variant.Dialog:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.Blazor\\Components\\Pages\\{context}\\{entityCollection}\\Components";
										case Variant.Select:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.Blazor\\Components\\Pages\\{context}\\{entityCollection}\\Components";
										case Variant.Page:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.Blazor\\Components\\Pages\\{context}\\{entityCollection}\\Components";
										case Variant.Form:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.Blazor\\Components\\Pages\\{context}\\{entityCollection}\\Components";
										case Variant.Table:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.Blazor\\Components\\Pages\\{context}\\{entityCollection}\\Components";
										case Variant.PageIndex:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.Blazor\\Components\\Pages\\{context}\\{entityCollection}";
										case Variant.PageCreate:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.Blazor\\Components\\Pages\\{context}\\{entityCollection}";
										case Variant.PageEdit:
											return $"src\\{task.Target.Project.Name}.Presentations.Web.Blazor\\Components\\Pages\\{context}\\{entityCollection}";
										default:
											throw new InvalidOperationException();
									}
								}
							default:
								throw new InvalidOperationException();
						}
					}
				default:
					throw new InvalidOperationException();
			}

			throw new InvalidOperationException();
		}
		private static string ResolveFileName(
			ScaffoldTask task)
		{
			var context = task.Target.ContextName;
			var entity = task.Target.EntityName;
			var entityCollection = entity?.PluralizeAsPascal();
			switch (task.Layer)
			{
				case Layer.Application:
					{
						switch (task.Type)
						{
							case ArtifactType.Command:
								{
									switch (task.Variant)
									{
										case Variant.New:
											return $"Patch{task.Target.Name}{entity}Command.cs";
										case Variant.Post:
											return $"Post{entity}Command.cs";
										case Variant.Put:
											return $"Put{entity}Command.cs";
										case Variant.Patch:
											return $"Patch{entity}Command.cs";
										case Variant.Delete:
											return $"Delete{entity}Command.cs";
										default:
											throw new InvalidOperationException();
									}
								}
							case ArtifactType.Query:
								{
									switch (task.Variant)
									{
										case Variant.GetById:
											return $"Get{entity}ByIdQuery.cs";
										case Variant.GetByFilter:
											return $"Get{entityCollection}ByFilterQuery.cs";
										default:
											throw new InvalidOperationException();
									}
								}
							case ArtifactType.Notification:
								{
									switch (task.Variant)
									{
										case Variant.New:
											return $"Patch{task.Target.Name}{entity}Notification.cs";
										case Variant.Post:
											return $"Post{entity}Notification.cs";
										case Variant.Put:
											return $"Put{entity}Notification.cs";
										case Variant.Patch:
											return $"Patch{entity}Notification.cs";
										case Variant.Delete:
											return $"Delete{entity}Notification.cs";
										default:
											throw new InvalidOperationException();
									}
								}
							default:
								throw new InvalidOperationException();
						}
					}
				case Layer.Domain:
					{
						switch (task.Type)
						{
							case ArtifactType.Entity:
								return $"{entity}.cs";
							case ArtifactType.EntityResource:
								{
									switch (task.Variant)
									{
										case Variant.Culture:
											return $"Entities{entityCollection}.pt-BR.resx";
										case Variant.Designer:
											return $"Entities{entityCollection}.{task.Variant}.cs";
										case Variant.Resource:
											return $"Entities{entityCollection}.resx";
										default:
											throw new InvalidOperationException();
									}
								}
							case ArtifactType.Service:
								{
									switch (task.Variant)
									{
										case Variant.New:
											return $"{task.Target.Name}{entity}Service.cs";
										case Variant.Create:
											return $"Create{entity}Service.cs";
										case Variant.Update:
											return $"Update{entity}Service.cs";
										case Variant.Delete:
											return $"Delete{entity}Service.cs";
										default:
											throw new InvalidOperationException();
									}
								}
							case ArtifactType.Validation:
								{
									switch (task.Variant)
									{
										case Variant.Entity:
											return $"{entity}Validator.cs";
										case Variant.New:
											return $"{task.Target.Name}{entity}SpecificationValidator.cs";
										case Variant.Create:
											return $"Create{entity}SpecificationValidator.cs";
										case Variant.Update:
											return $"Update{entity}SpecificationValidator.cs";
										case Variant.Delete:
											return $"Delete{entity}SpecificationValidator.cs";
										default:
											throw new InvalidOperationException();
									}
								}
							case ArtifactType.Specification:
								{
									switch (task.Variant)
									{
										case Variant.New:
											return $"{entity}{task.Target.Name}Specification.cs";
										default:
											throw new InvalidOperationException();
									}
								}
							case ArtifactType.ContextResource:
								{
									switch (task.Variant)
									{
										case Variant.Culture:
											return $"Context{context}.pt-BR.resx";
										case Variant.Designer:
											return $"Context{context}.{task.Variant}.cs";
										case Variant.Resource:
											return $"Context{context}.resx";
										default:
											throw new InvalidOperationException();
									}
								}
							case ArtifactType.Resource:
								{
									switch (task.Variant)
									{
										case Variant.Culture:
											return $"Mensagens.pt-BR.resx";
										case Variant.Designer:
											return $"Mensagens.{task.Variant}.cs";
										case Variant.Resource:
											return $"Mensagens.resx";
										default:
											throw new InvalidOperationException();
									}
								}
							case ArtifactType.IService:
								return $"I{task.Target.Name}Service.cs";
							case ArtifactType.IDbContextReader:
								return $"I{context}DbContextReader.cs";
							case ArtifactType.IDbContextWriter:
								return $"I{context}DbContextWriter.cs";
							default:
								throw new InvalidOperationException();
						}
					}
				case Layer.Infrastructure:
					{
						switch (task.Type)
						{
							case ArtifactType.Mapping:
								return $"{entity}Mapping.cs";
							case ArtifactType.DbContext:
								return $"{context}DbContext.cs";
							case ArtifactType.DbContextReader:
								return $"{context}DbContextReader.cs";
							case ArtifactType.DbContextWriter:
								return $"{context}DbContextWriter.cs";
							case ArtifactType.Service:
								return $"{task.Target.Name}Service.cs";
							default:
								throw new InvalidOperationException();
						}

					}
				case Layer.Middleware:
					{
						switch (task.Type)
						{
							case ArtifactType.DbContextConfigurations:
								return $"AddDbContextConfigurations.cs";
							case ArtifactType.DomainServicesConfigurations:
								return $"AddDomainServicesConfigurations.cs";
							case ArtifactType.ValidationsConfigurations:
								return $"AddValidationsConfigurations.cs";
							case ArtifactType.Configurations:
								return $"Configurations.cs";
							default:
								throw new InvalidOperationException();
						}
					}
				case Layer.Web:
					{
						switch (task.Type)
						{
							case ArtifactType.Api:
								{
									switch (task.Variant)
									{
										case Variant.Controller:
											return $"{entityCollection}Controller.cs";
										default:
											throw new InvalidOperationException();
									}
								}
							case ArtifactType.React:
								{
									switch (task.Variant)
									{
										case Variant.Index:
											return $"index.js";
										case Variant.Tab:
											return $"Tabs{entity}.js";
										case Variant.Form:
											return $"Form{entity}.js";
										case Variant.Table:
											return $"Table{entityCollection}.js";
										case Variant.PageIndex:
											return $"PageIndex.js";
										case Variant.PageCreate:
											return $"PageCreate.js";
										case Variant.PageEdit:
											return $"PageEdit.js";
										default:
											throw new InvalidOperationException();
									}
								}
							case ArtifactType.Blazor:
								{
									switch (task.Variant)
									{
										case Variant.Filter:
											return $"Filter{entityCollection}Helper.cs";
										case Variant.Dialog:
											return $"Dialog{entity}.razor";
										case Variant.Select:
											return $"Select{entity}.razor";
										case Variant.Page:
											return $"Page{entity}.razor";
										case Variant.Form:
											return $"Form{entity}.razor";
										case Variant.Table:
											return $"Table{entityCollection}.razor";
										case Variant.PageIndex:
											return $"Index.razor";
										case Variant.PageCreate:
											return $"Create.razor";
										case Variant.PageEdit:
											return $"Edit.razor";
										default:
											throw new InvalidOperationException();
									}
								}
							default:
								throw new InvalidOperationException();
						}
					}
				default:
					throw new InvalidOperationException();
			}

			throw new InvalidOperationException();
		}
		private static string ResolveTemplateKey(ScaffoldTask task)
		{
			if (task.Variant is Variant.None)
				return $"{task.Layer}.{task.Type}";

			return $"{task.Layer}.{task.Type}.{task.Variant}";
		}
	}
}
