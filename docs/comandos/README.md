# Referência de Comandos

Esta seção documenta todos os comandos disponíveis na ferramenta `forge`.

## Pré-requisito comum

A maioria dos comandos exige que o projeto esteja inicializado — ou seja, que exista o arquivo `.forge/project.json` no diretório atual.

**Exceções:** `forge init project` e `forge load sql` (que pode criar ou substituir o projeto).

## Índice de comandos

| Grupo | Comando | Documentação |
|-------|---------|--------------|
| Inicialização | `forge init project` | [init.md](init.md) |
| Adição | `forge add context` | [add.md](add.md) |
| Adição | `forge add entity` | [add.md](add.md) |
| Adição | `forge add property` | [add.md](add.md) |
| Adição | `forge add relation` | [add.md](add.md) |
| Atualização | `forge update context` | [update.md](update.md) |
| Atualização | `forge update entity` | [update.md](update.md) |
| Atualização | `forge update property` | [update.md](update.md) |
| Atualização | `forge update relation` | [update.md](update.md) |
| Remoção | `forge remove context` | [remove.md](remove.md) |
| Remoção | `forge remove entity` | [remove.md](remove.md) |
| Remoção | `forge remove property` | [remove.md](remove.md) |
| Remoção | `forge remove relation` | [remove.md](remove.md) |
| Listagem | `forge list all` | [list.md](list.md) |
| Geração | `forge scaffold` | [scaffold.md](scaffold.md) |
| Importação | `forge scan markers` | [scan.md](scan.md) |
| Importação | `forge load sql` | [load.md](load.md) |

## Sintaxe geral

```bash
forge <grupo> <subcomando> [argumentos] [opções]
```

As palavras-chave literais (`on`, `to`, `from`) fazem parte da sintaxe e devem ser digitadas exatamente como mostrado.

## Ajuda integrada

```bash
forge --help
forge add --help
forge add entity --help
forge scaffold --help
```
