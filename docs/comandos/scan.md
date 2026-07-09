# forge scan markers

Escaneia o código-fonte em busca de marcadores Forge (`<forge:*>`) e atualiza o `.forge/project.json` com os dados encontrados.

## Sintaxe

```bash
forge scan markers [opções]
```

## Opções

| Opção | Tipo | Padrão | Descrição |
|-------|------|--------|-----------|
| `--path` | `-p` | diretório atual | Raiz do scan |
| `--extensions` | `-e` | `.cs` | Extensões de arquivo (separadas por vírgula) |
| `--overwrite-entities` | bool | `false` | Sobrescrever entidades existentes |
| `--overwrite-properties` | bool | `false` | Sobrescrever propriedades existentes |
| `--overwrite-relations` | bool | `false` | Sobrescrever relações existentes |
| `--overwrite-all` | bool | `false` | Sobrescrever tudo (preset) |
| `--no-create-contexts` | bool | `false` | Não criar contextos ausentes |
| `--no-create-entities` | bool | `false` | Não criar entidades ausentes |
| `--no-create-properties` | bool | `false` | Não criar propriedades ausentes |
| `--no-create-relations` | bool | `false` | Não criar relações ausentes |
| `--dry-run` | bool | `false` | Simular sem salvar |

## Marcadores suportados

Os marcadores são comentários XML inseridos no código-fonte C#:

### Entidade

```csharp
// <forge:entity context="Vendas" name="Produto" description="Produto do catálogo" aggregateRoot="true" auditable="true">
public class Produto { }
```

| Atributo | Obrigatório | Padrão | Descrição |
|----------|-------------|--------|-----------|
| `context` | sim | — | Nome do contexto |
| `name` | sim | — | Nome da entidade |
| `description` | não | — | Descrição |
| `id-type` | não | `Guid` | Tipo do ID |
| `table` | não | — | Nome da tabela |
| `aggregateRoot` | não | — | `true`/`false` |
| `auditable` | não | — | `true`/`false` |

### Propriedade

```csharp
// <forge:property entity="Produto" name="Nome" type="string" required="true" length="128" context="Vendas">
public string Nome { get; set; }
```

| Atributo | Obrigatório | Padrão | Descrição |
|----------|-------------|--------|-----------|
| `entity` | sim | — | Nome da entidade |
| `name` | sim | — | Nome da propriedade |
| `type` | sim | — | Tipo CLR |
| `context` | não | — | Contexto (se diferente do padrão) |
| `required` | não | — | `true`/`false` |
| `length` | não | — | Comprimento |
| `has-max-length` | não | — | `true`/`false` |
| `precision` | não | — | Precisão decimal |
| `scale` | não | — | Escala decimal |
| `db-column` | não | — | Nome da coluna |
| `display-on-select` | não | — | `true`/`false` |

### Relação

```csharp
// <forge:relationship from="Produto" to="Categoria" kind="one-to-many" required="true" context="Vendas">
```

| Atributo | Obrigatório | Padrão | Descrição |
|----------|-------------|--------|-----------|
| `from` | sim | — | Entidade origem |
| `to` | sim | — | Entidade destino |
| `kind` | sim | — | `one-to-many` ou `many-to-one` |
| `context` | não | — | Contexto |
| `required` | não | — | `true`/`false` |

## Comportamento

1. Varre recursivamente o diretório especificado
2. Ignora pastas: `bin`, `obj`, `.git`, `node_modules`, `.forge`
3. Extrai marcadores `<forge:*>` dos arquivos
4. Agrega e converte para modelo Forge
5. Faz merge com o projeto existente (ou cria novo)
6. Salva em `.forge/project.json`

### Modos de merge

| Configuração | Comportamento |
|--------------|---------------|
| Padrão | Adiciona itens novos, mantém existentes |
| `--overwrite-all` | Substitui tudo com dados do scan |
| `--overwrite-entities` | Substitui apenas entidades |
| `--no-create-*` | Não cria itens ausentes do tipo especificado |

## Exemplos

```bash
# Scan básico no diretório atual
forge scan markers

# Scan em diretório específico
forge scan markers --path ./src

# Scan com múltiplas extensões
forge scan markers -e .cs,.cshtml

# Simular sem salvar
forge scan markers --dry-run

# Sobrescrever entidades existentes
forge scan markers --overwrite-entities

# Sobrescrever tudo
forge scan markers --overwrite-all

# Apenas adicionar novos itens (não criar contextos)
forge scan markers --no-create-contexts
```

## Exemplo completo no código

```csharp
namespace MeuApp.Core.Domain.Vendas.Produto.Entity
{
    // <forge:entity context="Vendas" name="Produto" auditable="true">
    public class Produto
    {
        // <forge:property entity="Produto" name="Nome" type="string" required="true" length="128" context="Vendas">
        public string Nome { get; set; }

        // <forge:property entity="Produto" name="Preco" type="decimal" required="true" precision="18" scale="2" context="Vendas">
        public decimal Preco { get; set; }

        // <forge:relationship from="Produto" to="Categoria" kind="many-to-one" required="true" context="Vendas">
        public Guid CategoriaId { get; set; }
    }
}
```

Depois de adicionar os marcadores:

```bash
forge scan markers
forge list all
```

## Saídas possíveis

| Mensagem | Significado |
|----------|-------------|
| `Project updated successfully.` | Modelo atualizado e salvo |
| `No markers found in scanned files.` | Nenhum marcador encontrado |
| `No changes to save.` | Marcadores encontrados, mas sem alterações |
| `[DRY-RUN] No changes were saved.` | Simulação concluída |
| `Scan completed with N parse error(s).` | Erros de parsing nos marcadores |

## Erros comuns

| Situação | Causa |
|----------|-------|
| `Tipo desconhecido` | Marcador com tipo inválido (use `entity`, `property`, `relationship`) |
| `Marcador inválido` | Sintaxe XML incorreta no marcador |
| Atributos ausentes | Faltam atributos obrigatórios (`context`, `name`, etc.) |
