# Documentação Forge.CLI

O **Forge** é um gerador de projetos orientado a modelo, baseado em **contextos**, **entidades**, **propriedades** e **relações**. A ferramenta de linha de comando `forge` permite definir o modelo do domínio e gerar código automaticamente para múltiplas camadas da aplicação.

## Índice

| Documento | Descrição |
|-----------|-----------|
| [Instalação](instalacao.md) | Como compilar, empacotar e instalar a ferramenta global |
| [Conceitos](conceitos.md) | Contextos, entidades, propriedades, relações e fluxo de trabalho |
| [Modelo do Projeto](modelo-projeto.md) | Estrutura do arquivo `.forge/project.json` |
| [Comandos](comandos/README.md) | Referência completa de todos os comandos CLI |
| [Arquitetura](arquitetura.md) | Estrutura interna do projeto e pipeline de scaffolding |
| [Exemplos](exemplos.md) | Fluxos completos de uso do início ao fim |

## Visão geral dos comandos

```
forge
├── init
│   ├── project                           # Inicializa o projeto Forge
│   ├── templates                         # Copia templates embutidos para .forge/Templates
│   └── artifacts                         # Copia artefatos embutidos para .forge/Artifacts
├── add
│   ├── context <context>                 # Adiciona um contexto
│   ├── entity <entity> on <context>      # Adiciona uma entidade
│   ├── property <prop> to <entity> on <context>   # Adiciona propriedade
│   └── relation <target> to <source> on <context> # Adiciona relação
├── update
│   ├── context <context>                 # Atualiza contexto
│   ├── entity <entity> on <context>      # Atualiza entidade
│   ├── property <prop> on <entity> <context>      # Atualiza propriedade
│   └── relation <target> from <source> on <context> # Atualiza relação
├── remove
│   ├── context <context>                 # Remove contexto
│   ├── entity <entity> from <context>    # Remove entidade
│   ├── property <prop> from <entity> on <context> # Remove propriedade
│   └── relation <target> from <source> on <context> # Remove relação
├── list all                              # Lista o modelo atual
├── scaffold [layer] [type] [variant]     # Gera código a partir do modelo
├── scan markers                          # Importa modelo de marcadores no código
└── load sql <file>                       # Importa modelo de script SQL
```

## Início rápido

```bash
# 1. Inicializar o projeto
forge init project --name MeuApp

# 2. Definir o modelo
forge add context Vendas
forge add entity Produto on Vendas
forge add property Nome to Produto on Vendas --type string --required
forge add property Preco to Produto on Vendas --type decimal --precision 18 --scale 2

# 3. Visualizar o modelo
forge list all

# 4. Gerar código
forge scaffold Domain Entity -c Vendas -e Produto --force --yes
```

## Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) compatível com `net10.0`
- Projeto inicializado com `forge init project` (exceto para `init` e `load sql`)

## Códigos de saída

| Código | Significado |
|--------|-------------|
| `0` | Sucesso |
| `1` | Erro (ex.: conflitos no scaffold, arquivo não encontrado) |
| `-1` | Erro (ex.: projeto não inicializado, item não encontrado) |
