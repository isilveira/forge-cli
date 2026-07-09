# Instalação

O Forge.CLI é distribuído como uma ferramenta global do .NET (`dotnet tool`).

## Compilar a partir do código-fonte

```bash
# Limpar artefatos anteriores
dotnet clean

# Compilar em Release
dotnet build -c Release

# Gerar o pacote NuGet da ferramenta
dotnet pack -c Release
```

O pacote será gerado em `src/Forge.CLI/bin/Release/`.

## Instalar como ferramenta global

```bash
# Desinstalar versão anterior (se existir)
dotnet tool uninstall forge.cli --global

# Instalar a partir do pacote local
dotnet tool install --global --add-source ./src/Forge.CLI/bin/Release Forge.CLI
```

Após a instalação, o comando `forge` estará disponível em qualquer diretório do terminal.

## Verificar a instalação

```bash
forge --help
```

## Atualizar a ferramenta

Após recompilar o projeto, reinstale a ferramenta:

```bash
dotnet tool uninstall forge.cli --global
dotnet tool install --global --add-source ./src/Forge.CLI/bin/Release Forge.CLI
```

## Desenvolvimento local (sem instalar globalmente)

Durante o desenvolvimento, é possível executar diretamente:

```bash
dotnet run --project src/Forge.CLI -- init project --name MeuApp
```

Ou usar o perfil de debug configurado em `launchSettings.json`:

```bash
forge scaffold --force --yes
```

## Requisitos do sistema

| Requisito | Versão |
|-----------|--------|
| .NET SDK | `net10.0` ou superior |
| Sistema operacional | Windows, Linux ou macOS |
