# forge add

Comandos para adicionar elementos ao modelo Forge. Todos exigem projeto inicializado.

---

## forge add context

Adiciona um novo contexto (bounded context) ao projeto.

### Sintaxe

```bash
forge add context <contexto> [opções]
```

### Argumentos

| Argumento | Descrição |
|-----------|-----------|
| `<contexto>` | Nome do contexto (obrigatório) |

### Opções

| Opção | Alias | Tipo | Padrão | Descrição |
|-------|-------|------|--------|-----------|
| `--schema` | `-s` | string | `{contexto}Db` | Schema do banco de dados |
| `--description` | `-d` | string | — | Descrição do contexto |

### Exemplos

```bash
# Contexto simples
forge add context Vendas

# Com schema e descrição
forge add context Estoque -s EstoqueDb -d "Gestão de estoque e armazéns"

# Múltiplos contextos
forge add context Vendas
forge add context Catalogo
forge add context Financeiro
```

---

## forge add entity

Adiciona uma entidade a um contexto existente.

### Sintaxe

```bash
forge add entity <entidade> on <contexto> [opções]
```

### Argumentos

| Argumento | Descrição |
|-----------|-----------|
| `<entidade>` | Nome da entidade |
| `on` | Palavra-chave literal (obrigatória) |
| `<contexto>` | Nome do contexto alvo |

### Opções

| Opção | Alias | Tipo | Padrão | Descrição |
|-------|-------|------|--------|-----------|
| `--table` | `-t` | string | pluralizado | Nome da tabela no banco |
| `--description` | `-d` | string | — | Descrição da entidade |
| `--aggregate-root` | — | bool | `true` | Se é raiz de agregação |
| `--auditable` | — | bool | `true` | Se suporta auditoria |

### Exemplos

```bash
# Entidade básica (tabela pluralizada automaticamente: Produtos)
forge add entity Produto on Vendas

# Com nome de tabela customizado
forge add entity Produto on Vendas --table TB_Produtos

# Entidade não auditável
forge add entity LogEvento on Vendas --auditable false

# Entidade que não é aggregate root
forge add entity ItemPedido on Vendas --aggregate-root false
```

---

## forge add property

Adiciona uma propriedade a uma entidade existente.

### Sintaxe

```bash
forge add property <propriedade> to <entidade> on <contexto> [opções]
```

### Argumentos

| Argumento | Descrição |
|-----------|-----------|
| `<propriedade>` | Nome da propriedade |
| `to` | Palavra-chave literal |
| `<entidade>` | Nome da entidade |
| `on` | Palavra-chave literal |
| `<contexto>` | Nome do contexto |

### Opções

| Opção | Alias | Tipo | Padrão | Descrição |
|-------|-------|------|--------|-----------|
| `--type` | `-t` | string | `string` | Tipo CLR da propriedade |
| `--required` | — | bool | `false` | Campo obrigatório |
| `--length` | — | int | `128` | Comprimento máximo (strings) |
| `--has-max-length` | — | bool | `false` | Aplicar atributo MaxLength |
| `--precision` | — | int | `18` | Precisão (decimal) |
| `--scale` | — | int | `2` | Escala (decimal) |
| `--db-column` | — | string | nome da propriedade | Nome da coluna no banco |
| `--display-on-select` | — | bool | `false` | Exibir em componentes de seleção |

### Exemplos

```bash
# Propriedade string obrigatória
forge add property Nome to Produto on Vendas --type string --required

# String com comprimento customizado
forge add property Codigo to Produto on Vendas --type string --length 20 --has-max-length

# Valor decimal (preço)
forge add property Preco to Produto on Vendas --type decimal --precision 18 --scale 2 --required

# Inteiro
forge add property Quantidade to Produto on Vendas --type int --required

# Booleano
forge add property Ativo to Produto on Vendas --type bool

# Data
forge add property DataCriacao to Produto on Vendas --type DateTime

# Coluna com nome diferente no banco
forge add property Descricao to Produto on Vendas --type string --db-column DS_PRODUTO

# Propriedade exibida em dropdowns
forge add property Nome to Produto on Vendas --type string --display-on-select
```

### Tipos suportados

`string`, `int`, `long`, `decimal`, `bool`, `Guid`, `DateTime`, `DateTimeOffset`

---

## forge add relation

Adiciona uma relação entre duas entidades. Cria automaticamente a relação inversa.

### Sintaxe

```bash
forge add relation <entidadeDestino> to <entidadeOrigem> on <contexto> [opções]
```

### Argumentos

| Argumento | Descrição |
|-----------|-----------|
| `<entidadeDestino>` | Entidade referenciada (lado "um") |
| `to` | Palavra-chave literal |
| `<entidadeOrigem>` | Entidade que recebe a FK (lado "muitos") |
| `on` | Palavra-chave literal |
| `<contexto>` | Nome do contexto |

### Opções

| Opção | Alias | Tipo | Padrão | Descrição |
|-------|-------|------|--------|-----------|
| `--name` | `-n` | string | nome do destino | Nome da relação na entidade origem |
| `--required` | — | bool | `false` | FK obrigatória |

### Comportamento

Ao executar `forge add relation Categoria to Produto on Vendas`:

1. Em **Produto**: cria relação `many-to-one` → `Categoria`
2. Em **Categoria**: cria relação `one-to-many` → `Produtos` (nome pluralizado)

### Exemplos

```bash
# Relação simples: Produto pertence a uma Categoria
forge add relation Categoria to Produto on Vendas

# FK obrigatória
forge add relation Categoria to Produto on Vendas --required

# Com nome customizado para a relação
forge add relation Cliente to Pedido on Vendas --name Comprador --required

# Múltiplas relações
forge add relation Categoria to Produto on Vendas --required
forge add relation Fornecedor to Produto on Vendas
forge add relation Produto to ItemPedido on Vendas --required
```

### Diagrama

```
Categoria (1) ←──── (N) Produto
   ↑                      ↑
   │                      │
one-to-many          many-to-one
(em Categoria)       (em Produto)
```

## Erros comuns

| Mensagem | Causa |
|----------|-------|
| `Forge not inicialized` | Execute `forge init project` primeiro |
| `Context 'X' not found` | Contexto não existe — use `forge add context` |
| `Entity 'X' not found` | Entidade não existe no contexto |
| `already exists` | Item já existe no modelo |
