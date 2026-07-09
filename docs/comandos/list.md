# forge list all

Exibe o modelo Forge atual em formato de árvore no terminal.

## Sintaxe

```bash
forge list all [opções]
```

## Opções

| Opção | Alias | Tipo | Descrição |
|-------|-------|------|-----------|
| `--context` | `-c` | string | Filtra por contexto específico |
| `--entity` | `-e` | string | Filtra por entidade específica |

## Comportamento

Exibe uma árvore hierárquica usando Spectre.Console:

```
Vendas
├── Produto
│   ├── Nome (string)
│   ├── Preco (decimal)
│   └── relation: Categoria (many-to-one)
├── Categoria
│   ├── Nome (string)
│   └── relation: Produtos (one-to-many)
└── Pedido
    ├── DataPedido (DateTime)
    └── relation: Cliente (many-to-one)
```

Para cada entidade, são listadas:
- **Propriedades** — com nome e tipo CLR
- **Relações** — com nome e tipo (`one-to-many` ou `many-to-one`)

## Exemplos

```bash
# Listar todo o modelo
forge list all

# Filtrar por contexto
forge list all --context Vendas
forge list all -c Vendas

# Filtrar por entidade
forge list all --entity Produto
forge list all -e Produto

# Combinar filtros
forge list all -c Vendas -e Produto
```

## Saída quando vazio

Se não houver contextos:

```
No contexts found.
```

## Erros comuns

| Mensagem | Causa |
|----------|-------|
| `Forge not inicialized` | Execute `forge init project` primeiro |
