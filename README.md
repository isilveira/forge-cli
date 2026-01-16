# forge-cli
Forge is a model-driven project generator based on contexts, entities and properties.

## Development Commands
> cd src/Forge.CLI

> dotnet clean

> dotnet build -c Release

> dotnet pack -c Release

> dotnet tool uninstall forge.cli --global

> dotnet tool install --global --add-source ./bin/Release Forge.CLI

## Usage Commands
> forge init project -n|--name MyProject


> forge add context MyContext

> forge add entity MyEntity on MyContext

> forge add property MyProperty to MyEntity on MyContext --type string --required true (--maxLength 100 | --precision 18 --scale 2)

> forge add relation TargetEntity to SourceEntity on MyContext --relationType one-to-many --required true


> forge remove context MyContext

> forge remove entity MyEntity from MyContext

> forge remove property MyProperty from MyEntity on MyContext

> forge remove relation TargetEntity from SourceEntity on MyContext


> forge list all --context MyContext --entity MyEntity

## Architecture
Forge.CLI
│
├─ Commands
│   ├─ Init
│   │   └─ InitProjectCommand.cs
│   ├─ Add
│   │   ├─ AddContextCommand.cs
│   │   ├─ AddEntityCommand.cs
│   │   ├─ AddPropertyCommand.cs
│   │   └─ AddRelationCommand.cs
│   ├─ Remove
│   │   ├─ RemoveContextCommand.cs
│   │   ├─ RemoveEntityCommand.cs
│   │   └─ RemovePropertyCommand.cs
│   ├─ List
│   │   └─ ListCommand.cs
│   └─ Scaffold
│       └─ ScaffoldCommand.cs
│
├─ Core
│   ├─ Capabilities
│   │   ├─ Layer.cs
│   │   ├─ ArtifactType.cs
│   │   ├─ Variant.cs
│   │   ├─ ArtifactCapability.cs
│   │   ├─ LayerCapability.cs
│   │   └─ CapabilityMatrix.cs
│   │
│   ├─ Targets
│   │   ├─ TargetScope.cs
│   │   ├─ ScaffoldTarget.cs
│   │   └─ TargetResolver.cs
│   │
│   ├─ Planning
│   │   ├─ ScaffoldRequest.cs
│   │   ├─ ScaffoldTask.cs
│   │   ├─ ScaffoldPlan.cs
│   │   └─ ScaffoldPlanner.cs
│   │
│   ├─ Artifacts
│   │   ├─ ArtifactDescriptor.cs
│   │   └─ ArtifactDescriptorResolver.cs
│   │
│   ├─ Templates
│   │   ├─ TemplateDefinition.cs
│   │   ├─ ITemplateResolver.cs
│   │   ├─ TemplateResolver.cs
│   │   ├─ TemplateModel.cs
│   │   ├─ TemplateModelBuilder.cs
│   │   ├─ ITemplateRenderer.cs
│   │   └─ ScribanTemplateRenderer.cs
│   │
│   └─ Execution
│       ├─ RenderedArtifact.cs
│       ├─ ExecutionOptions.cs
│       ├─ IFileSystem.cs
│       ├─ PhysicalFileSystem.cs
│       ├─ PathResolver.cs
│       ├─ OverwritePolicy.cs
│       └─ ScaffoldExecutor.cs
│
├─ Models
│   ├─ ForgeProject.cs
│   ├─ ForgeContext.cs
│   ├─ ForgeEntity.cs
│   ├─ ForgeProperty.cs
│   └─ ForgeRelation.cs
│
├─ Persistence
│   ├─ ProjectLoader.cs
│   └─ ProjectSaver.cs
│
├─ Shared
│   └─ Helpers
│       └─ AnsiConsoleHelper.cs
│
├─ Templates
│   ├─ Domain
│   │   ├─ Entity.tpl
│   │   ├─ Service.Create.tpl
│   │   └─ Validation.Entity.tpl
│   ├─ Application
│   │   ├─ Command.Post.tpl
│   │   └─ Query.GetById.tpl
│   └─ Infrastructure
│       └─ Mapping.tpl
│
└─ Program.cs

## Scaffold Flow
ScaffoldCommand
 → ScaffoldRequest
   → TargetResolver
     → ScaffoldPlanner
       → ScaffoldTask[]
         → ArtifactDescriptorResolver
           → TemplateResolver
             → TemplateRenderer
               → RenderedArtifact
                 → ScaffoldExecutor

## Templates Structure
Scriban Templates Structure:
/Scaffolding
  /Templates
    /Domain
      /Entity
        entity.sbn

Intepolated string Structure:
/Scaffolding
  /Templates
    /Domain
      /Entity
        EntityTemplate.cs

Razor Templates Structure:
Scaffolding
 ├── Templates
 │   ├── Domain
 │   │   └── Entity
 │   │       └── Entity.cshtml
 │   ├── Application
 │   │   └── Command
 │   │       └── Create.cshtml
 │   └── Infrastructure
 │       └── EntityMapping.cshtml
 ├── Rendering
 │   ├── IRazorRenderer.cs
 │   └── RazorRenderer.cs
 └── Models
     └── EntityTemplateModel.cs