# Conceitos

O Forge modela aplicações em uma hierarquia de quatro níveis. Entender esses conceitos é essencial para usar a ferramenta de forma eficaz.

## Hierarquia do modelo

```
Projeto (ForgeProject)
└── Contexto (ForgeContext)          ← bounded context / schema de banco
    └── Entidade (ForgeEntity)       ← tabela / aggregate root
        ├── Propriedade (ForgeProperty)   ← coluna / campo
        └── Relação (ForgeRelation)       ← FK / navegação
```

## Projeto

O **projeto Forge** é a unidade raiz. Ele é persistido no arquivo `.forge/project.json` no diretório de trabalho atual.

Contém configurações globais como:
- Nome do projeto
- Tipo padrão de ID (`Guid`, `int`, `string`, etc.)
- Convenções de nomenclatura e caminhos
- Lista de exceções para o scaffolding

## Contexto

Um **contexto** representa um bounded context ou schema de banco de dados. Cada contexto agrupa entidades relacionadas.

Exemplos:
- `Vendas` — pedidos, produtos, clientes
- `Estoque` — armazéns, movimentações
- `dbo` — contexto padrão ao importar de SQL sem schema explícito

Propriedades principais:
- **Schema** — nome do schema no banco (padrão: `{Contexto}Db`)
- **Description** — descrição textual

## Entidade

Uma **entidade** mapeia para uma tabela no banco e uma classe no domínio.

Propriedades principais:
- **Table** — nome da tabela (pluralizado automaticamente se a convenção estiver ativa)
- **IdType** — tipo da chave primária (herda `DefaultIdType` do projeto)
- **AggregateRoot** — se a entidade é raiz de agregação (padrão: `true`)
- **Auditable** — se a entidade suporta auditoria (padrão: `true`)

## Propriedade

Uma **propriedade** representa um campo/coluna da entidade.

Tipos suportados:

| Tipo CLR | Uso típico | Opções relevantes |
|----------|------------|-------------------|
| `string` | Texto | `--length`, `--has-max-length` |
| `int` | Inteiro | — |
| `long` | Inteiro longo | — |
| `decimal` | Valores monetários | `--precision`, `--scale` |
| `bool` | Booleano | — |
| `Guid` | Identificadores | — |
| `DateTime` | Data/hora | — |
| `DateTimeOffset` | Data/hora com fuso | — |

Opções adicionais:
- **Required** — campo obrigatório
- **DbColumn** — nome da coluna no banco (padrão: nome da propriedade)
- **DisplayOnSelect** — exibir em componentes de seleção (dropdowns)
- **DisplayOnTable** — exibir em tabelas (padrão: `true`)

## Relação

Uma **relação** define o vínculo entre duas entidades.

Tipos:
- `many-to-one` — muitos registros de uma entidade referenciam um registro de outra (lado da FK)
- `one-to-many` — um registro referenciado por muitos (lado inverso)

Ao adicionar uma relação com `forge add relation`, o Forge cria automaticamente:
1. Uma relação `many-to-one` na entidade **origem** (source)
2. Uma relação `one-to-many` inversa na entidade **destino** (target)

**Exemplo:** `forge add relation Categoria to Produto on Vendas` cria:
- Em `Produto`: relação `many-to-one` → `Categoria`
- Em `Categoria`: relação `one-to-many` → `Produtos` (nome pluralizado)

## Camadas de scaffolding

O Forge gera código organizado em camadas:

| Camada | Descrição | Exemplos gerados |
|--------|-----------|------------------|
| **Domain** | Entidades, serviços, validações, recursos | `Entity`, `Service`, `Validation` |
| **Application** | CQRS — comandos, queries, notificações | `Command`, `Query`, `Notification` |
| **Infrastructure** | Persistência e implementações | `DbContext`, `Mapping`, `Service` |
| **Middleware** | Configurações de DI | `Configurations`, `DbContextConfigurations` |
| **Web** | Interface e API | `Api/Controller`, `Blazor/Page`, `Blazor/Form` |

## Fluxos de trabalho

### Modelo manual (CLI)

Defina o modelo passo a passo com os comandos `add`:

```
init → add context → add entity → add property → add relation → scaffold
```

### Importar de SQL

Carregue um script `CREATE TABLE` (estilo migrations do Entity Framework):

```
load sql script.sql → scaffold
```

### Importar de marcadores no código

Use marcadores XML no código-fonte e sincronize com `scan markers`:

```csharp
// <forge:entity context="Vendas" name="Produto" auditable="true">
// <forge:property entity="Produto" name="Nome" type="string" required="true" context="Vendas">
```

```
scan markers → scaffold
```

### Modelo híbrido

Combine qualquer um dos fluxos acima. O arquivo `.forge/project.json` é a fonte de verdade do modelo.
