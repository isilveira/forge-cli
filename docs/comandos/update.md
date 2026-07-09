# forge update

Comandos para atualizar elementos existentes no modelo Forge. Apenas os campos informados nas opções são alterados.

---

## forge update context

Atualiza um contexto existente.

### Sintaxe

```bash
forge update context <contexto> [opções]
```

### Opções

| Opção | Alias | Tipo | Descrição |
|-------|-------|------|-----------|
| `--name` | — | string | Novo nome do contexto (renomeia) |
| `--schema` | `-s` | string | Novo schema do banco |
| `--description` | `-d` | string | Nova descrição |

### Exemplos

```bash
# Atualizar schema
forge update context Vendas --schema SalesDb

# Atualizar descrição
forge update context Vendas -d "Módulo de vendas e pedidos"

# Renomear contexto
forge update context Vendas --name Comercial

# Múltiplas alterações
forge update context Vendas --schema VendasDb -d "Gestão comercial"
```

---

## forge update entity

Atualiza uma entidade existente.

### Sintaxe

```bash
forge update entity <entidade> on <contexto> [opções]
```

### Opções

| Opção | Alias | Tipo | Descrição |
|-------|-------|------|-----------|
| `--name` | — | string | Novo nome da entidade (renomeia) |
| `--table` | `-t` | string | Novo nome da tabela |
| `--description` | `-d` | string | Nova descrição |
| `--aggregate-root` | — | bool? | Altera se é aggregate root |
| `--auditable` | — | bool? | Altera se é auditável |

### Exemplos

```bash
# Renomear tabela
forge update entity Produto on Vendas --table TB_Produto

# Desativar auditoria
forge update entity LogEvento on Vendas --auditable false

# Renomear entidade
forge update entity Produto on Vendas --name ItemCatalogo

# Atualizar descrição
forge update entity Produto on Vendas -d "Item do catálogo de produtos"
```

---

## forge update property

Atualiza uma propriedade existente.

### Sintaxe

```bash
forge update property <propriedade> on <entidade> <contexto> [opções]
```

> **Atenção:** A sintaxe usa `<propriedade> on <entidade> <contexto>` — sem a palavra-chave `on` antes do contexto.

### Opções

| Opção | Tipo | Descrição |
|-------|------|-----------|
| `--name` | string | Novo nome da propriedade (renomeia) |
| `--type` | string | Novo tipo CLR |
| `--required` | bool? | Altera obrigatoriedade |
| `--length` | int? | Novo comprimento |
| `--has-max-length` | bool? | Altera MaxLength |
| `--precision` | int? | Nova precisão |
| `--scale` | int? | Nova escala |
| `--db-column` | string | Novo nome da coluna |
| `--display-on-select` | bool | Define exibição em selects (só aplica se `true`) |

### Exemplos

```bash
# Alterar tipo
forge update property Codigo on Produto Vendas --type int

# Tornar obrigatório
forge update property Descricao on Produto Vendas --required true

# Ajustar precisão decimal
forge update property Preco on Produto Vendas --precision 10 --scale 4

# Renomear propriedade
forge update property Nome on Produto Vendas --name Titulo

# Alterar coluna no banco
forge update property Nome on Produto Vendas --db-column DS_NOME
```

---

## forge update relation

Atualiza uma relação existente na entidade origem.

### Sintaxe

```bash
forge update relation <entidadeDestino> from <entidadeOrigem> on <contexto> [opções]
```

### Opções

| Opção | Tipo | Descrição |
|-------|------|-----------|
| `--type` | string | Novo tipo (`one-to-many`, `many-to-one`) |
| `--required` | bool? | Altera obrigatoriedade da FK |

### Exemplos

```bash
# Tornar FK obrigatória
forge update relation Categoria from Produto on Vendas --required true

# Alterar tipo da relação
forge update relation Categoria from Produto on Vendas --type many-to-one
```

> **Nota:** A relação é localizada pela chave `targetEntity` no dicionário de relações da entidade origem. A relação inversa no destino **não** é atualizada automaticamente.
