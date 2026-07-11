# Forge.CLI

Gerador de projetos orientado a modelo para aplicações .NET. O **Forge** permite definir o domínio em um arquivo central (`.forge/project.json`) e gerar código automaticamente para múltiplas camadas — Domain, Application, Infrastructure, Middleware e Web.

## O que é

O Forge modela aplicações em uma hierarquia simples:

```
Projeto → Contexto → Entidade → Propriedade / Relação
```

A partir desse modelo, a ferramenta `forge` gera entidades, serviços, validações, comandos CQRS, DbContext, controllers de API, páginas Blazor e muito mais.

O modelo pode ser definido de três formas:

- **CLI** — comandos `add`, `update` e `remove`
- **SQL** — importação de scripts `CREATE TABLE` (estilo migrations do EF)
- **Marcadores** — anotações `<forge:*>` no código-fonte, sincronizadas com `scan markers`

## Tecnologias

| Componente | Tecnologia |
|------------|------------|
| Runtime | .NET `net10.0` |
| CLI | [Spectre.Console.Cli](https://spectreconsole.net/) |
| Templates | [RazorLight](https://github.com/toddams/RazorLight) |
| Artefatos | YAML + [YamlDotNet](https://github.com/aaubry/YamlDotNet) |
| Validação gerada | [FluentValidation](https://fluentvalidation.net/) |

Distribuído como `dotnet tool` global — o comando instalado é `forge`.

## Início rápido

```bash
# Instalar a ferramenta (após build) — use --source (não --add-source)
dotnet build -c Release
dotnet tool install --global --source ./src/Forge.CLI/bin/Release isilveira.Forge.CLI

# Criar e modelar um projeto
forge init project --name MeuApp
forge add context Vendas
forge add entity Produto on Vendas
forge add property Nome to Produto on Vendas --type string --required

# Gerar código
forge scaffold --force --yes
```

## Documentação

A documentação completa está em [`docs/`](docs/README.md):

| Documento | Conteúdo |
|-----------|----------|
| [Instalação](docs/instalacao.md) | Compilar, empacotar e instalar |
| [Conceitos](docs/conceitos.md) | Modelo, camadas e fluxos de trabalho |
| [Modelo do Projeto](docs/modelo-projeto.md) | Schema do `.forge/project.json` |
| [Comandos](docs/comandos/README.md) | Referência de todos os comandos e opções |
| [Scaffold](docs/comandos/scaffold.md) | Catálogo de artefatos gerados |
| [Exemplos](docs/exemplos.md) | Fluxos completos do início ao fim |
| [Arquitetura](docs/arquitetura.md) | Estrutura interna do projeto |

## Desenvolvimento

```bash
dotnet clean
dotnet build -c Release
dotnet pack -c Release
```

Para reinstalar após alterações:

```bash
dotnet tool uninstall isilveira.Forge.CLI --global
dotnet tool install --global --source ./src/Forge.CLI/bin/Release isilveira.Forge.CLI
```
