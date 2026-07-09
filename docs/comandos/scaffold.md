# forge scaffold

Gera código-fonte a partir do modelo Forge definido em `.forge/project.json`. Utiliza templates Razor embutidos e definições de artefatos YAML.

## Sintaxe

```bash
forge scaffold [camada] [tipo] [variante] [opções]
```

Todos os argumentos posicionais são opcionais. Sem argumentos, o scaffold tenta gerar todos os artefatos aplicáveis.

## Argumentos posicionais

| Argumento | Descrição | Exemplos |
|-----------|-----------|----------|
| `[camada]` | Camada da aplicação | `Domain`, `Application`, `Infrastructure`, `Middleware`, `Web` |
| `[tipo]` | Tipo do artefato | `Entity`, `Command`, `Blazor`, `Api`, etc. |
| `[variante]` | Variante do artefato | `New`, `Post`, `PageIndex`, `Create`, etc. |

## Opções

| Opção | Alias | Tipo | Padrão | Descrição |
|-------|-------|------|--------|-----------|
| `--context` | `-c` | string | todos | Filtra por contexto |
| `--entity` | `-e` | string | todas | Filtra por entidade |
| `--name` | `-n` | string | `New` | Nome usado em templates "New" |
| `--what-if` | — | bool | `false` | Simulação — não grava arquivos |
| `--force` | — | bool | `false` | Sobrescreve arquivos existentes |
| `--yes` | — | bool | `false` | Não pede confirmação por arquivo |

## Comportamento

1. Carrega `.forge/project.json`
2. Descobre artefatos YAML em `Scaffolding/Artifacts/` (ou recursos embutidos)
3. `ScaffoldPlanner` filtra por camada/tipo/variante e resolve alvos (contexto/entidade)
4. Renderiza templates Razor (`.cshtml`)
5. `FileSystemExecutor` grava os arquivos no disco

### Filtro de variantes

Se a variante **não** for informada, artefatos com variante `New` são **excluídos** por padrão. Para gerá-los, especifique explicitamente:

```bash
forge scaffold Domain Service New -c Vendas -e Produto
```

### Estratégias de sobrescrita

| Situação | Comportamento |
|----------|---------------|
| Arquivo não existe | Cria o arquivo |
| Arquivo existe, sem `--force` | Gera conflito (exit code `1`) |
| Arquivo existe, com `--force` | Sobrescreve o arquivo |

### Confirmação por arquivo

Sem `--yes`, o Forge pede confirmação para cada arquivo gerado. Use `--yes` em scripts e CI.

## Exemplos

```bash
# Gerar tudo (com sobrescrita e sem confirmação)
forge scaffold --force --yes

# Simular geração completa (não grava)
forge scaffold --what-if

# Gerar apenas entidades de domínio
forge scaffold Domain Entity --force --yes

# Gerar para contexto e entidade específicos
forge scaffold Domain Entity -c Vendas -e Produto --force

# Gerar camada Application (comandos e queries)
forge scaffold Application -c Vendas -e Produto --force --yes

# Gerar página Blazor de listagem
forge scaffold Web Blazor PageIndex -c Vendas -e Produto --force

# Gerar controller de API
forge scaffold Web Api Controller -c Vendas -e Produto --force

# Gerar infraestrutura (DbContext, Mapping)
forge scaffold Infrastructure -c Vendas --force --yes

# Gerar configurações de DI
forge scaffold Middleware --force --yes

# Gerar serviço de domínio (variante New)
forge scaffold Domain Service New -c Vendas -e Produto --force

# Gerar validações
forge scaffold Domain Validation Entity -c Vendas -e Produto --force
```

## Catálogo de artefatos

### Domain

| Tipo | Variantes | Descrição |
|------|-----------|-----------|
| `Entity` | — | Classe de entidade de domínio |
| `Service` | `New`, `Create`, `Update`, `Delete` | Serviços de domínio |
| `IService` | `New` | Interface de serviço |
| `Validation` | `New`, `Entity`, `Create`, `Update`, `Delete` | Validadores FluentValidation |
| `Specification` | `New` | Especificações de domínio |
| `Resource` | `Resource`, `Designer`, `Culture` | Recursos de localização |
| `EntityResource` | `Resource`, `Designer`, `Culture` | Recursos por entidade |
| `ContextResource` | `Resource`, `Designer`, `Culture` | Recursos por contexto |
| `IDbContextReader` | — | Interface de leitura |
| `IDbContextWriter` | — | Interface de escrita |

### Application

| Tipo | Variantes | Descrição |
|------|-----------|-----------|
| `Command` | `New`, `Post`, `Put`, `Patch`, `Delete` | Comandos CQRS |
| `Query` | `New`, `GetById`, `GetByFilter` | Queries CQRS |
| `Notification` | `New`, `Post`, `Put`, `Patch`, `Delete` | Handlers de notificação |

### Infrastructure

| Tipo | Variantes | Descrição |
|------|-----------|-----------|
| `DbContext` | — | DbContext do Entity Framework |
| `DbContextReader` | — | Implementação de leitura |
| `DbContextWriter` | — | Implementação de escrita |
| `Mapping` | — | Configuração EF (Fluent API) |
| `Service` | `New` | Implementação de serviço |

### Middleware

| Tipo | Variantes | Descrição |
|------|-----------|-----------|
| `Configurations` | — | Configurações gerais de DI |
| `DbContextConfigurations` | — | Registro de DbContexts |
| `DomainServicesConfigurations` | — | Registro de serviços de domínio |
| `ValidationsConfigurations` | — | Registro de validadores |

### Web

| Tipo | Variantes | Descrição |
|------|-----------|-----------|
| `Api` | `Controller` | Controller REST |
| `Blazor` | `Page`, `PageIndex`, `PageCreate`, `PageEdit`, `Form`, `Table`, `Filter`, `Select`, `Dialog`, `Menu` | Componentes Blazor |

## Exclusões via project.json

Configure `ScaffoldExceptions` no `.forge/project.json` para excluir artefatos:

```json
{
  "ScaffoldExceptions": [
    "Web",
    "Domain.Validation.New"
  ]
}
```

## Estrutura de saída (exemplo)

Para `forge scaffold Domain Entity -c Vendas -e Produto`:

```
src/
└── MeuApp.Core.Domain/
    └── Vendas/
        └── Produto/
            └── Entity/
                └── Produto.cs
```

Os caminhos seguem os padrões definidos nos artefatos YAML e nas convenções do projeto.

## Erros comuns

| Situação | Causa | Solução |
|----------|-------|---------|
| Exit code `1` com conflitos | Arquivos já existem | Use `--force` |
| `Projeto não inicializado` | Sem `.forge/project.json` | Execute `forge init project` |
| Artefato ignorado (skip) | Falta contexto/entidade alvo | Passe `-c` e `-e` |
