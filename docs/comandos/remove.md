# forge remove

Comandos para remover elementos do modelo Forge. Todos pedem confirmação interativa, exceto quando `--force` é usado.

---

## forge remove context

Remove um contexto e todas as suas entidades.

### Sintaxe

```bash
forge remove context <contexto> [opções]
```

### Opções

| Opção | Alias | Tipo | Padrão | Descrição |
|-------|-------|------|--------|-----------|
| `--force` | `-f` | bool | `false` | Remove sem pedir confirmação |

### Exemplos

```bash
# Com confirmação interativa
forge remove context Vendas

# Sem confirmação
forge remove context Vendas --force
forge remove context Vendas -f
```

---

## forge remove entity

Remove uma entidade de um contexto.

### Sintaxe

```bash
forge remove entity <entidade> from <contexto> [opções]
```

### Argumentos

| Argumento | Descrição |
|-----------|-----------|
| `<entidade>` | Nome da entidade |
| `from` | Palavra-chave literal |
| `<contexto>` | Nome do contexto |

### Opções

| Opção | Alias | Descrição |
|-------|-------|-----------|
| `--force` | `-f` | Remove sem confirmação |

### Exemplos

```bash
forge remove entity Produto from Vendas
forge remove entity LogEvento from Vendas -f
```

---

## forge remove property

Remove uma propriedade de uma entidade.

### Sintaxe

```bash
forge remove property <propriedade> from <entidade> on <contexto> [opções]
```

### Argumentos

| Argumento | Descrição |
|-----------|-----------|
| `<propriedade>` | Nome da propriedade |
| `from` | Palavra-chave literal |
| `<entidade>` | Nome da entidade |
| `on` | Palavra-chave literal |
| `<contexto>` | Nome do contexto |

### Opções

| Opção | Alias | Descrição |
|-------|-------|-----------|
| `--force` | `-f` | Remove sem confirmação |

### Exemplos

```bash
forge remove property Descricao from Produto on Vendas
forge remove property PrecoAntigo from Produto on Vendas -f
```

---

## forge remove relation

Remove uma relação da entidade origem.

### Sintaxe

```bash
forge remove relation <entidadeDestino> from <entidadeOrigem> on <contexto> [opções]
```

### Argumentos

| Argumento | Descrição |
|-----------|-----------|
| `<entidadeDestino>` | Entidade referenciada |
| `from` | Palavra-chave literal |
| `<entidadeOrigem>` | Entidade que possui a FK |
| `on` | Palavra-chave literal |
| `<contexto>` | Nome do contexto |

### Opções

| Opção | Alias | Descrição |
|-------|-------|-----------|
| `--force` | `-f` | Remove sem confirmação |

### Comportamento

Remove **apenas** a relação na entidade origem. A relação inversa na entidade destino **não** é removida automaticamente.

### Exemplos

```bash
forge remove relation Categoria from Produto on Vendas
forge remove relation Fornecedor from Produto on Vendas -f
```

## Confirmação interativa

Sem `--force`, o Forge exibe uma pergunta de confirmação:

```
Are you sure you want to remove the context 'Vendas'?
```

Responda `y` para confirmar ou `n` para cancelar. A operação cancelada retorna código `0` sem alterar o modelo.
