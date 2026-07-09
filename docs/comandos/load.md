# forge load sql

Carrega o modelo do projeto a partir de um script SQL com instruções `CREATE TABLE` (estilo migrations do Entity Framework).

## Sintaxe

```bash
forge load sql <arquivo> [opções]
```

## Argumentos

| Argumento | Descrição |
|-----------|-----------|
| `<arquivo>` | Caminho do script SQL (obrigatório) |

## Opções

| Opção | Alias | Tipo | Padrão | Descrição |
|-------|-------|------|--------|-----------|
| `--context` | `-c` | string | por schema SQL | Força um único contexto para todas as tabelas |
| `--project-name` | `-n` | string | nome da pasta | Nome do projeto |
| `--merge` | — | bool | `false` | Faz merge com projeto existente |
| `--merge-add-only` | — | bool | `false` | Merge apenas adicionando novos itens *(declarado, não implementado)* |
| `--merge-overwrite-all` | — | bool | `false` | Merge sobrescrevendo tudo *(declarado, não implementado)* |
| `--dry-run` | — | bool | `false` | Simula sem gravar |

## Comportamento

1. Lê e faz parse do script SQL
2. Extrai tabelas (`CREATE TABLE`), colunas e chaves estrangeiras
3. Converte para modelo Forge:
   - Cada tabela → entidade (nome singularizado)
   - Cada coluna → propriedade (tipo mapeado de SQL para CLR)
   - Cada FK → relação `many-to-one`
4. Salva ou faz merge com `.forge/project.json`

### Mapeamento de contextos

| Situação | Resultado |
|----------|-----------|
| Sem `--context` | Cada schema SQL vira um contexto (`dbo`, `Sales`, etc.) |
| Com `--context Vendas` | Todas as tabelas vão para o contexto `Vendas` |

### Modos de operação

| Modo | Comportamento |
|------|---------------|
| Padrão (sem `--merge`) | **Substitui** o modelo inteiro |
| Com `--merge` | Faz merge com projeto existente (sobrescreve itens conflitantes) |
| Com `--dry-run` | Exibe resultado sem gravar |

## Mapeamento de tipos SQL → CLR

| Tipo SQL | Tipo CLR |
|----------|----------|
| `NVARCHAR`, `VARCHAR`, `TEXT` | `string` |
| `INT` | `int` |
| `BIGINT` | `long` |
| `DECIMAL`, `NUMERIC` | `decimal` |
| `BIT` | `bool` |
| `UNIQUEIDENTIFIER` | `Guid` |
| `DATETIME`, `DATETIME2` | `DateTime` |
| `DATETIMEOFFSET` | `DateTimeOffset` |

## Exemplos

```bash
# Carregar de migration do EF
forge load sql migrations/001_InitialCreate.sql

# Forçar contexto único
forge load sql script.sql --context Vendas

# Com nome de projeto
forge load sql script.sql --context Vendas --project-name MeuApp

# Merge com projeto existente
forge load sql novas_tabelas.sql --merge

# Simular sem gravar
forge load sql script.sql --dry-run

# Combinar opções
forge load sql migrations/001_InitialCreate.sql -c Vendas -n MeuApp --merge
```

## Exemplo de script SQL suportado

```sql
CREATE TABLE [Sales].[Categories] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(128) NOT NULL
);

CREATE TABLE [Sales].[Products] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(256) NOT NULL,
    [Price] DECIMAL(18, 2) NOT NULL,
    [CategoryId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_Products_Categories] FOREIGN KEY ([CategoryId])
        REFERENCES [Sales].[Categories] ([Id])
);
```

Resultado no modelo:

```
Sales (contexto)
├── Category
│   └── Name (string)
└── Product
    ├── Name (string)
    ├── Price (decimal)
    └── relation: Category (many-to-one)
```

## Fluxo de saída

```
[LOAD] reading SQL file: migrations/001_InitialCreate.sql
[PARSE] parsing SQL script...
[PARSE] found 5 table(s)
[CONVERT] building ForgeProject...
[CONVERT] 5 entity(ies), 23 property(ies), 4 relation(s)
[SAVE] writing project.json...
Project loaded successfully from SQL script.
```

Com `--merge`:

```
[MERGE] merging with existing project...
[MERGE] merge completed
[SAVE] writing project.json...
```

## Erros comuns

| Mensagem | Causa | Solução |
|----------|-------|---------|
| `File not found` | Caminho inválido | Verifique o caminho do arquivo |
| `SQL file is empty` | Arquivo vazio | Verifique o conteúdo |
| `No CREATE TABLE statements found` | Script sem tabelas | Use formato `CREATE TABLE` |
| `Parse error` | SQL malformado | Verifique sintaxe do script |

## Após o carregamento

```bash
# Verificar o modelo importado
forge list all

# Gerar código
forge scaffold --force --yes
```
