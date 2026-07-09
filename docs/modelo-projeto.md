# Modelo do Projeto

O modelo Forge é persistido em `.forge/project.json` no diretório raiz do projeto.

## Localização

```
meu-projeto/
├── .forge/
│   └── project.json      ← modelo Forge
├── src/
│   └── ...
└── ...
```

> **Nota:** O arquivo de configuração é `.forge/project.json`, não `forge.json`.

## Estrutura completa

```json
{
  "SchemaVersion": "1.0",
  "Name": "MeuApp",
  "DefaultIdType": "Guid",
  "UseSealedClasses": true,
  "UseVirtualCollections": false,
  "Tab": "    ",
  "DefaultConventions": {
    "UsePluralizedTables": true,
    "UseDefaultValueOnStringIds": true,
    "DefaultStringIdLength": "36",
    "DefaultProject": "{projectName}",
    "DefaultProjectPath": "{projectName}",
    "DefaultContext": "{contextName}",
    "DefaultContextPath": "{contextName}",
    "DefaultEntity": "{entityName}",
    "DefaultEntityPath": "{entityName}",
    "DefaultEntityPluralized": true
  },
  "Contexts": {
    "Vendas": {
      "Schema": "VendasDb",
      "Description": "Contexto de vendas",
      "Entities": {
        "Produto": {
          "IdType": "Guid",
          "Table": "Produtos",
          "Description": "Produto do catálogo",
          "AggregateRoot": true,
          "Auditable": true,
          "Properties": {
            "Nome": {
              "Type": "string",
              "Required": true,
              "Length": 128,
              "HasMaxLength": false,
              "Precision": null,
              "Scale": null,
              "DbColumn": "Nome",
              "DisplayOnSelect": true,
              "DisplayOnTable": true
            },
            "Preco": {
              "Type": "decimal",
              "Required": true,
              "Length": null,
              "Precision": 18,
              "Scale": 2,
              "DbColumn": "Preco",
              "DisplayOnSelect": false,
              "DisplayOnTable": true
            }
          },
          "Relations": {
            "Categoria": {
              "Type": "many-to-one",
              "Target": "Categoria",
              "Required": true
            }
          }
        }
      }
    }
  },
  "ScaffoldExceptions": []
}
```

## Campos do projeto

### Nível raiz (`ForgeProject`)

| Campo | Tipo | Padrão | Descrição |
|-------|------|--------|-----------|
| `SchemaVersion` | string | `"1.0"` | Versão do schema JSON |
| `Name` | string | — | Nome do projeto |
| `DefaultIdType` | string | `"Guid"` | Tipo padrão de ID para novas entidades |
| `UseSealedClasses` | bool | `true` | Gerar classes `sealed` |
| `UseVirtualCollections` | bool | `false` | Coleções de navegação virtuais |
| `Tab` | string | `"    "` | Indentação usada nos templates |
| `DefaultConventions` | object | — | Convenções de nomenclatura e caminhos |
| `Contexts` | object | `{}` | Dicionário de contextos |
| `ScaffoldExceptions` | array | `[]` | Artefatos excluídos do scaffolding |

### Convenções (`DefaultConventions`)

| Campo | Padrão | Descrição |
|-------|--------|-----------|
| `UsePluralizedTables` | `true` | Pluralizar nomes de tabela automaticamente |
| `UseDefaultValueOnStringIds` | `true` | Valor padrão em IDs do tipo string |
| `DefaultStringIdLength` | `"36"` | Comprimento padrão de IDs string |
| `DefaultProject` | `"{projectName}"` | Padrão de namespace do projeto |
| `DefaultProjectPath` | `"{projectName}"` | Padrão de caminho do projeto |
| `DefaultContext` | `"{contextName}"` | Padrão de namespace do contexto |
| `DefaultContextPath` | `"{contextName}"` | Padrão de caminho do contexto |
| `DefaultEntity` | `"{entityName}"` | Padrão de namespace da entidade |
| `DefaultEntityPath` | `"{entityName}"` | Padrão de caminho da entidade |
| `DefaultEntityPluralized` | `true` | Pluralizar entidade nos caminhos |

### Contexto (`ForgeContext`)

| Campo | Tipo | Padrão | Descrição |
|-------|------|--------|-----------|
| `Schema` | string | `"{contextName}Db"` | Schema do banco de dados |
| `Description` | string | gerada | Descrição do contexto |
| `Entities` | object | `{}` | Dicionário de entidades |

### Entidade (`ForgeEntity`)

| Campo | Tipo | Padrão | Descrição |
|-------|------|--------|-----------|
| `IdType` | string | `DefaultIdType` | Tipo da chave primária |
| `Table` | string | pluralizado | Nome da tabela |
| `Description` | string | gerada | Descrição da entidade |
| `AggregateRoot` | bool | `true` | É raiz de agregação |
| `Auditable` | bool | `true` | Suporta auditoria |
| `Properties` | object | `{}` | Propriedades da entidade |
| `Relations` | object | `{}` | Relações da entidade |

### Propriedade (`ForgeProperty`)

| Campo | Tipo | Padrão | Descrição |
|-------|------|--------|-----------|
| `Type` | string | — | Tipo CLR (`string`, `int`, `decimal`, etc.) |
| `Required` | bool | `false` | Campo obrigatório |
| `Length` | int? | `null` | Comprimento máximo (apenas `string`) |
| `HasMaxLength` | bool | `false` | Aplicar `MaxLength` |
| `Precision` | int? | `null` | Precisão (apenas `decimal`) |
| `Scale` | int? | `null` | Escala (apenas `decimal`) |
| `DbColumn` | string | nome da propriedade | Nome da coluna no banco |
| `DisplayOnSelect` | bool | `false` | Exibir em selects/dropdowns |
| `DisplayOnTable` | bool | `true` | Exibir em tabelas |

### Relação (`ForgeRelation`)

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Type` | string | `one-to-many` ou `many-to-one` |
| `Target` | string | Nome da entidade alvo |
| `Required` | bool | FK obrigatória |

## Exclusões de scaffolding

O campo `ScaffoldExceptions` permite excluir artefatos específicos da geração:

```json
{
  "ScaffoldExceptions": [
    "Web",
    "Web.Blazor",
    "Domain.Validation.New"
  ]
}
```

| Padrão | Efeito |
|--------|--------|
| `"Domain"` | Exclui toda a camada Domain |
| `"Domain.Entity"` | Exclui apenas artefatos do tipo Entity |
| `"Domain.Entity.New"` | Exclui variante específica |

## Normalização automática (`Sharpen`)

Ao salvar ou carregar o projeto, o Forge normaliza automaticamente:

- Tipos de ID e propriedades são mapeados (`guid` → `Guid`, `datetime` → `DateTime`)
- `Length` é removido de tipos que não suportam comprimento
- `Precision`/`Scale` são removidos de tipos que não suportam
- `DbColumn` recebe o nome da propriedade se estiver vazio
- `Table` é pluralizado se a convenção estiver ativa
- `Schema` e `Description` recebem valores padrão se vazios

## Edição manual

O arquivo pode ser editado manualmente, mas é recomendado usar os comandos CLI (`add`, `update`, `remove`) para evitar inconsistências. Após edição manual, execute `forge list all` para validar o modelo.
