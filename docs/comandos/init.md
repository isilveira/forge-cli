# forge init project

Inicializa um novo projeto Forge no diretório atual, criando o arquivo `.forge/project.json`.

## Sintaxe

```bash
forge init project [opções]
```

## Opções

| Opção | Alias | Tipo | Padrão | Descrição |
|-------|-------|------|--------|-----------|
| `--name` | `-n` | string | nome da pasta atual | Nome do projeto Forge |
| `--default-id-typed` | `-d` | string | `Guid` | Tipo padrão de ID das entidades |

## Comportamento

1. Verifica se o projeto já está inicializado — falha se `.forge/project.json` já existir
2. Cria um `ForgeProject` com `SchemaVersion = "1.0"` e convenções padrão
3. Salva em `.forge/project.json`
4. Exibe mensagem de sucesso

## Exemplos

```bash
# Inicializar com nome padrão (nome da pasta atual)
forge init project

# Inicializar com nome específico
forge init project --name MeuApp

# Inicializar com tipo de ID inteiro
forge init project --name MeuApp --default-id-typed int

# Forma abreviada
forge init project -n MeuApp -d Guid
```

## Tipos de ID suportados

| Valor | Descrição |
|-------|-----------|
| `Guid` | Identificador único global (padrão) |
| `int` | Inteiro auto-incremento |
| `long` | Inteiro longo |
| `string` | Texto (comprimento configurável via convenções) |

## Erros comuns

| Mensagem | Causa | Solução |
|----------|-------|---------|
| Projeto já inicializado | `.forge/project.json` já existe | Remova o arquivo ou use outro diretório |

## Próximos passos

Após inicializar, adicione contextos e entidades:

```bash
forge add context Vendas
forge add entity Produto on Vendas
```
