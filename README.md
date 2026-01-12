# forge-cli
Forge is a model-driven project generator based on contexts, entities and properties.

## Development Commands
> cd src/Forge.CLI

> dotnet clean

> dotnet build -c Release

> dotnet pack -c Release

> dotnet tool uninstall forge.cli --global

> dotnet tool install --global --add-source ./bin/Release Forge.CLI

## Usage Commands
> forge init project -n|--name MyProject


> forge add context MyContext

> forge add entity MyEntity on MyContext

> forge add property MyProperty to MyEntity on MyContext --type string --required true (--maxLength 100 | --precision 18 --scale 2)

> forge add relation TargetEntity to SourceEntity on MyContext --relationType one-to-many --required true


> forge remove context MyContext

> forge remove entity MyEntity from MyContext

> forge remove property MyProperty from MyEntity on MyContext

> forge remove relation TargetEntity from SourceEntity on MyContext


> forge list all --context MyContext --entity MyEntity