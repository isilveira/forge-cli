# forge init

Comandos de inicialização do Forge no diretório atual.

---

## forge init project

Inicializa um novo projeto Forge no diretório atual, criando o arquivo `.forge/project.json`.

### Sintaxe

```bash
forge init project [opções]
```

### Opções

| Opção | Alias | Tipo | Padrão | Descrição |
|-------|-------|------|--------|-----------|
| `--name` | `-n` | string | nome da pasta atual | Nome do projeto Forge |
| `--default-id-typed` | `-d` | string | `Guid` | Tipo padrão de ID das entidades |

### Comportamento

1. Verifica se o projeto já está inicializado — falha se `.forge/project.json` já existir
2. Cria um `ForgeProject` com `SchemaVersion = "1.0"` e convenções padrão
3. Salva em `.forge/project.json`
4. Exibe mensagem de sucesso

### Exemplos

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

### Tipos de ID suportados

| Valor | Descrição |
|-------|-----------|
| `Guid` | Identificador único global (padrão) |
| `int` | Inteiro auto-incremento |
| `long` | Inteiro longo |
| `string` | Texto (comprimento configurável via convenções) |

### Erros comuns

| Mensagem | Causa | Solução |
|----------|-------|---------|
| Projeto já inicializado | `.forge/project.json` já existe | Remova o arquivo ou use outro diretório |

### Próximos passos

Após inicializar, adicione contextos e entidades:

```bash
forge add context Vendas
forge add entity Produto on Vendas
```

---

## forge init templates

Copia os templates Razor embutidos na ferramenta para a pasta local `.forge/Templates`, preservando a estrutura de pastas (`Domain/Entity.cshtml`, `Web/Blazor/Page.cshtml`, etc.).

### Sintaxe

```bash
forge init templates [opções]
```

### Opções

| Opção | Alias | Tipo | Padrão | Descrição |
|-------|-------|------|--------|-----------|
| `--what-if` | — | flag | `false` | Lista o que seria copiado, sem gravar arquivos |
| `--force` | — | flag | `false` | Sobrescreve templates que já existem |
| `--yes` | — | flag | `false` | Confirma sobrescrita sem perguntar (com `--force`) |

### Comportamento

1. Enumera os recursos embutidos `Scaffolding/Templates/**/*.cshtml`
2. Mapeia cada recurso para `.forge/Templates/<caminho relativo>`
3. Sem `--force`, copia apenas arquivos que ainda não existem
4. Com `--force`, sobrescreve os existentes (pede confirmação, a menos que `--yes`)
5. Com `--what-if`, apenas reporta o plano

Não exige que o projeto esteja inicializado — cria `.forge/Templates` se necessário.

### Exemplos

```bash
# Copiar templates ausentes
forge init templates

# Ver o que seria copiado
forge init templates --what-if

# Sobrescrever tudo sem confirmação
forge init templates --force --yes
```

### Destino

```
.forge/
└── Templates/
    ├── Domain/
    │   ├── Entity.cshtml
    │   └── Service/
    │       └── Create.cshtml
    ├── Application/
    ├── Infrastructure/
    ├── Middleware/
    └── Web/
```

---

## forge init artifacts

Copia os artefatos YAML embutidos na ferramenta para a pasta local `.forge/Artifacts`, preservando a estrutura de pastas (`Domain/entity.yaml`, `Application/Command/Delete.yaml`, etc.).

O `scaffold` prioriza artefatos em `.forge/Artifacts` (e em `Scaffolding/Artifacts`) sobre os embutidos quando o mesmo `id` existe nos dois lugares.

### Sintaxe

```bash
forge init artifacts [opções]
```

### Opções

| Opção | Alias | Tipo | Padrão | Descrição |
|-------|-------|------|--------|-----------|
| `--what-if` | — | flag | `false` | Lista o que seria copiado, sem gravar arquivos |
| `--force` | — | flag | `false` | Sobrescreve artefatos que já existem |
| `--yes` | — | flag | `false` | Confirma sobrescrita sem perguntar (com `--force`) |

### Comportamento

1. Enumera os recursos embutidos `Scaffolding/Artifacts/**/*.yaml`
2. Mapeia cada recurso para `.forge/Artifacts/<caminho relativo>`
3. Sem `--force`, copia apenas arquivos que ainda não existem
4. Com `--force`, sobrescreve os existentes (pede confirmação, a menos que `--yes`)
5. Com `--what-if`, apenas reporta o plano

Não exige que o projeto esteja inicializado — cria `.forge/Artifacts` se necessário.

### Exemplos

```bash
# Copiar artefatos ausentes
forge init artifacts

# Ver o que seria copiado
forge init artifacts --what-if

# Sobrescrever tudo sem confirmação
forge init artifacts --force --yes
```

### Destino

```
.forge/
└── Artifacts/
    ├── Domain/
    │   ├── entity.yaml
    │   └── Service/
    │       └── Create.yaml
    ├── Application/
    ├── Infrastructure/
    ├── Middleware/
    └── Web/
```
