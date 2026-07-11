# Instalação

O Forge.CLI é distribuído como uma ferramenta global do .NET (`dotnet tool`).

> **Atenção:** no nuget.org existe outro pacote chamado `Forge.Cli` (orquestrador de sprints). O pacote deste repositório é `isilveira.Forge.CLI`. Use sempre `--source` apontando para a pasta local do `.nupkg` — nunca só `--add-source`, que ainda consulta o nuget.org.

## Compilar a partir do código-fonte

Na raiz do repositório:

```bash
dotnet clean
dotnet build -c Release
dotnet pack -c Release
```

O pacote será gerado em `src/Forge.CLI/bin/Release/isilveira.Forge.CLI.1.1.0.nupkg`.

## Instalar como ferramenta global

Na raiz do repositório:

```bash
# Desinstalar versões anteriores (qualquer um dos IDs)
dotnet tool uninstall forge.cli --global 2>$null
dotnet tool uninstall isilveira.Forge.CLI --global 2>$null

# Instalar SOMENTE a partir do pacote local (--source substitui nuget.org)
dotnet tool install --global --source ./src/Forge.CLI/bin/Release isilveira.Forge.CLI
```

Se estiver em `src/Forge.CLI`:

```bash
dotnet tool install --global --source ./bin/Release isilveira.Forge.CLI
```

Após a instalação, o comando `forge` estará disponível em qualquer diretório do terminal.

## Verificar a instalação

```bash
forge --help
```

Os comandos esperados são: `init`, `add`, `update`, `remove`, `list`, `scaffold`, `scan`, `load`.

Se aparecerem `analyze`, `run`, `mcp`, etc., a ferramenta errada do nuget.org foi instalada — desinstale e use o comando com `--source` acima.

## Atualizar a ferramenta

Após recompilar o projeto:

```bash
dotnet tool uninstall isilveira.Forge.CLI --global
dotnet tool install --global --source ./src/Forge.CLI/bin/Release isilveira.Forge.CLI
```

## Desenvolvimento local (sem instalar globalmente)

Durante o desenvolvimento, é possível executar diretamente:

```bash
dotnet run --project src/Forge.CLI -- init project --name MeuApp
```

## Requisitos do sistema

| Requisito | Versão |
|-----------|--------|
| .NET SDK | `net10.0` ou superior |
| Sistema operacional | Windows, Linux ou macOS |
