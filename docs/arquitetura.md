# Arquitetura

Visão geral da estrutura interna do projeto Forge.CLI.

## Estrutura de diretórios

```
src/Forge.CLI/
├── Program.cs                          # Ponto de entrada
├── Forge.CLI.csproj                    # PackAsTool, ToolCommandName=forge
│
├── Registers/
│   └── ForgeRegister.cs                # Registro de todos os comandos CLI
│
├── Commands/                           # Handlers CLI por domínio
│   ├── Init/          InitProjectCommand.cs
│   ├── Add/           AddContext|Entity|Property|Relation
│   ├── Update/        UpdateContext|Entity|Property|Relation
│   ├── Remove/        RemoveContext|Entity|Property|Relation
│   ├── List/          ListCommand.cs
│   ├── Scaffold/      ScaffoldCommand.cs
│   ├── Scan/          ScanCommand.cs
│   ├── Load/          LoadSqlCommand.cs
│   └── *ForgeCommandGroup.cs           # Branches Spectre.Console.Cli
│
├── Models/                             # Modelo de domínio Forge
│   ├── ForgeProject.cs
│   ├── ForgeContext.cs
│   ├── ForgeEntity.cs
│   ├── ForgeProperty.cs
│   └── ForgeRelation.cs
│
├── Persistence/
│   ├── ProjectLoader.cs                # Leitura de .forge/project.json
│   └── ProjectSaver.cs                 # Gravação de .forge/project.json
│
├── Core/
│   ├── Artifacts/                      # Descoberta e validação de artefatos YAML
│   ├── Scaffolding/                    # Planejamento, conflitos, execução
│   ├── Templates/                      # Renderização Razor (RazorLight)
│   ├── CodeScanning/                   # Scan de marcadores <forge:*>
│   └── SqlLoading/                     # Parse SQL → ForgeProject
│
├── Scaffolding/
│   ├── Artifacts/**/*.yaml             # 56 definições de artefatos
│   └── Templates/**/*.cshtml           # Templates Razor embutidos
│
└── Shared/Helpers/                     # Utilitários compartilhados
```

## Stack tecnológica

| Componente | Tecnologia | Versão |
|------------|------------|--------|
| Runtime | .NET | `net10.0` |
| CLI Framework | Spectre.Console.Cli | 0.53.1 |
| Templates | RazorLight | 2.3.1 |
| Interpolação | Scriban | 6.5.2 |
| Artefatos | YamlDotNet | 16.3.0 |
| Validação | FluentValidation | 12.1.1 |
| Pluralização | BAYSOFT.Abstractions.Tools | 10.0.1.2 |

## Fluxo de scaffolding

```
ScaffoldCommand
  │
  ├─ 1. ProjectLoader.TryLoad()          → ForgeProject
  ├─ 2. ArtifactRegistryFactory        → 56 artefatos YAML
  ├─ 3. ScaffoldPlanner.BuildAsync()    → ScaffoldPlan
  │     ├─ FilterArtifacts (layer/type/variant)
  │     ├─ ResolveTargets (context/entity)
  │     ├─ RazorTemplateRenderer        → conteúdo renderizado
  │     └─ ConflictDetector             → detecta arquivos existentes
  │
  └─ 4. Executor
        ├─ DryRunExecutor (--what-if)
        └─ FileSystemExecutor (padrão)
```

## Fluxo de scan

```
ScanCommand
  │
  ├─ 1. FileScanner                     → varre diretório
  ├─ 2. MarkerParser                    → parse <forge:*>
  ├─ 3. MarkerAggregator                → agrega por contexto/entidade
  ├─ 4. ScannedProjectConverter         → ScannedProjectModel
  ├─ 5. ProjectMerger                   → merge com projeto existente
  └─ 6. ProjectSaver                    → grava .forge/project.json
```

## Fluxo de load SQL

```
LoadSqlCommand
  │
  ├─ 1. SqlScriptParser                 → ParsedSqlModel
  ├─ 2. SqlToForgeProjectConverter      → ForgeProject
  ├─ 3. ProjectMerger (se --merge)      → merge
  └─ 4. ProjectSaver                    → grava .forge/project.json
```

## Artefatos YAML

Cada artefato em `Scaffolding/Artifacts/` define:

```yaml
artifact:
  id: Domain.Entity
  version: 1

layer: Domain
type: Entity
variant: null
description: "Generates a domain entity class"

generation:
  enabled: true
  target:
    namespacePattern: "{{projectConvention}}.Core.Domain.{{contextConvention}}.{{entityConvention}}.Entity"
    path: "src\\{{projectConvention}}.Core.Domain\\{{contextConvention}}\\{{entityConvention}}\\Entity"
    filename: "{{entityName}}.cs"
  template:
    engine: razor
    file: "Domain/Entity"
```

Variáveis de interpolação:
- `{{projectName}}`, `{{contextName}}`, `{{entityName}}`
- `{{projectConvention}}`, `{{contextConvention}}`, `{{entityConvention}}`

## Templates Razor

Templates embutidos como `EmbeddedResource` no `.csproj`. Renderizados via RazorLight com um modelo (`TemplateModel`) que expõe o projeto, contexto, entidade e propriedades.

Estrutura:

```
Scaffolding/Templates/
├── Domain/
│   ├── Entity.cshtml
│   ├── Service/ (Create, Update, Delete, New)
│   └── Validation/ (Entity, Create, Update, Delete, New)
├── Application/
│   ├── Command/ (Post, Put, Patch, Delete, New)
│   ├── Query/ (GetById, GetByFilter, New)
│   └── Notification/ (Post, Put, Patch, Delete, New)
├── Infrastructure/
│   ├── DbContext.cshtml
│   ├── Mapping.cshtml
│   └── Service/New.cshtml
├── Middleware/
│   └── Configurations.cshtml
└── Web/
    ├── Api/Controller.cshtml
    ├── Blazor/ (Page, Form, Table, ...)
    └── React/ (templates existem, sem artefatos YAML)
```

## Registro de comandos

`ForgeRegister.cs` registra todos os grupos:

```csharp
InitForgeCommandGroup.Register(config);
AddForgeCommandGroup.Register(config);
UpdateForgeCommandGroup.Register(config);
RemoveForgeCommandGroup.Register(config);
ListForgeCommandGroup.Register(config);
ScaffoldForgeCommandGroup.Register(config);
ScanForgeCommandGroup.Register(config);
LoadForgeCommandGroup.Register(config);
```

## Persistência

O modelo é serializado/deserializado como JSON em `.forge/project.json`:

- **Leitura:** `ProjectLoader.TryLoad()` — retorna `null` se não existir
- **Gravação:** `ProjectSaver.SaveAsync()` — cria diretório `.forge/` se necessário
- **Normalização:** `ForgeProject.Sharpen()` — limpa e padroniza valores antes de salvar

## Tratamento de erros

`Program.cs` captura exceções não tratadas:

```csharp
catch (Exception ex)
{
    AnsiConsoleHelper.SafeMarkupLine($"Erro: {ex.Message}", "red");
    return -1;
}
```

Cada comando retorna códigos específicos conforme documentado na [referência de comandos](comandos/README.md).
