# Exemplos

Fluxos completos de uso do Forge.CLI, do início ao fim.

---

## Exemplo 1: CRUD de Produtos (modelo manual)

Criar um módulo de catálogo de produtos do zero.

### Passo 1 — Inicializar

```bash
mkdir MeuApp && cd MeuApp
forge init project --name MeuApp
```

### Passo 2 — Definir o modelo

```bash
# Contexto
forge add context Catalogo -d "Catálogo de produtos"

# Entidades
forge add entity Categoria on Catalogo
forge add entity Produto on Catalogo

# Propriedades da Categoria
forge add property Nome to Categoria on Catalogo --type string --required --length 128
forge add property Descricao to Categoria on Catalogo --type string --length 500

# Propriedades do Produto
forge add property Nome to Produto on Catalogo --type string --required --display-on-select
forge add property Preco to Produto on Catalogo --type decimal --precision 18 --scale 2 --required
forge add property Ativo to Produto on Catalogo --type bool
forge add property Estoque to Produto on Catalogo --type int --required

# Relação
forge add relation Categoria to Produto on Catalogo --required
```

### Passo 3 — Verificar

```bash
forge list all
```

### Passo 4 — Gerar código

```bash
# Gerar tudo
forge scaffold --force --yes

# Ou por camada
forge scaffold Domain -c Catalogo --force --yes
forge scaffold Application -c Catalogo -e Produto --force --yes
forge scaffold Infrastructure -c Catalogo --force --yes
forge scaffold Web Blazor PageIndex -c Catalogo -e Produto --force
```

---

## Exemplo 2: Importar de migration SQL

Carregar modelo a partir de um script de migration do Entity Framework.

### Script SQL (`migrations/001_Initial.sql`)

```sql
CREATE TABLE [dbo].[Customers] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(256) NOT NULL,
    [Email] NVARCHAR(256) NOT NULL
);

CREATE TABLE [dbo].[Orders] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [OrderDate] DATETIME2 NOT NULL,
    [Total] DECIMAL(18, 2) NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_Orders_Customers] FOREIGN KEY ([CustomerId])
        REFERENCES [dbo].[Customers] ([Id])
);

CREATE TABLE [dbo].[OrderItems] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Quantity] INT NOT NULL,
    [UnitPrice] DECIMAL(18, 2) NOT NULL,
    [OrderId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_OrderItems_Orders] FOREIGN KEY ([OrderId])
        REFERENCES [dbo].[Orders] ([Id])
);
```

### Comandos

```bash
# Inicializar (se necessário)
forge init project --name LojaOnline

# Carregar do SQL
forge load sql migrations/001_Initial.sql --context Vendas

# Verificar
forge list all -c Vendas

# Gerar
forge scaffold --force --yes
```

---

## Exemplo 3: Sincronizar com marcadores no código

Manter o modelo sincronizado com anotações no código-fonte.

### Código com marcadores

```csharp
// Arquivo: src/MeuApp.Core.Domain/Vendas/Cliente/Entity/Cliente.cs

namespace MeuApp.Core.Domain.Vendas.Cliente.Entity
{
    // <forge:entity context="Vendas" name="Cliente" auditable="true">
    public class Cliente
    {
        // <forge:property entity="Cliente" name="Nome" type="string" required="true" length="256" context="Vendas">
        public string Nome { get; set; }

        // <forge:property entity="Cliente" name="Email" type="string" required="true" context="Vendas">
        public string Email { get; set; }

        // <forge:property entity="Cliente" name="Telefone" type="string" context="Vendas">
        public string? Telefone { get; set; }
    }
}
```

### Comandos

```bash
# Escanear código
forge scan markers --path ./src

# Verificar modelo atualizado
forge list all -c Vendas -e Cliente

# Gerar camada Application
forge scaffold Application Command Post -c Vendas -e Cliente --force
forge scaffold Application Query GetById -c Vendas -e Cliente --force
```

---

## Exemplo 4: Scaffold seletivo por camada

Gerar apenas partes específicas da aplicação.

```bash
# Apenas entidades de domínio
forge scaffold Domain Entity --force --yes

# Apenas API REST para uma entidade
forge scaffold Web Api Controller -c Vendas -e Produto --force

# Apenas páginas Blazor
forge scaffold Web Blazor PageIndex -c Vendas -e Produto --force
forge scaffold Web Blazor PageCreate -c Vendas -e Produto --force
forge scaffold Web Blazor PageEdit -c Vendas -e Produto --force
forge scaffold Web Blazor Form -c Vendas -e Produto --force
forge scaffold Web Blazor Table -c Vendas -e Produto --force

# Apenas infraestrutura
forge scaffold Infrastructure DbContext -c Vendas --force
forge scaffold Infrastructure Mapping -c Vendas -e Produto --force

# Simular antes de gerar
forge scaffold Domain -c Vendas --what-if
```

---

## Exemplo 5: Atualizar modelo existente

Modificar um modelo já definido.

```bash
# Adicionar nova propriedade
forge add property SKU to Produto on Catalogo --type string --required --length 20

# Renomear propriedade
forge update property Nome on Produto Catalogo --name Titulo

# Adicionar nova entidade e relação
forge add entity Fornecedor on Catalogo
forge add property Nome to Fornecedor on Catalogo --type string --required
forge add relation Fornecedor to Produto on Catalogo

# Remover propriedade obsoleta
forge remove property Descricao from Produto on Catalogo -f

# Regenerar código
forge scaffold Domain Entity -c Catalogo -e Produto --force
forge scaffold Application -c Catalogo -e Produto --force
```

---

## Exemplo 6: Múltiplos contextos

Aplicação com bounded contexts separados.

```bash
forge init project --name ERP

# Contexto de Vendas
forge add context Vendas
forge add entity Pedido on Vendas
forge add entity Cliente on Vendas
forge add property Nome to Cliente on Vendas --type string --required
forge add property DataPedido to Pedido on Vendas --type DateTime --required
forge add relation Cliente to Pedido on Vendas --required

# Contexto de Estoque
forge add context Estoque
forge add entity Produto on Estoque
forge add entity Armazem on Estoque
forge add property Nome to Produto on Estoque --type string --required
forge add property Nome to Armazem on Estoque --type string --required

# Gerar por contexto
forge scaffold Domain -c Vendas --force --yes
forge scaffold Domain -c Estoque --force --yes
forge scaffold Infrastructure -c Vendas --force --yes
forge scaffold Infrastructure -c Estoque --force --yes
```

---

## Exemplo 7: Pipeline CI/CD

Script para automação em pipeline de integração contínua.

```bash
#!/bin/bash
set -e

# Carregar modelo do SQL
forge load sql db/schema.sql --context App --project-name MyApp

# Gerar todo o código
forge scaffold --force --yes

# Verificar se há conflitos (exit code != 0)
if [ $? -ne 0 ]; then
  echo "Scaffold failed with conflicts"
  exit 1
fi

echo "Code generation completed successfully"
```

---

## Exemplo 8: Excluir artefatos do scaffold

Configurar exclusões no `.forge/project.json`:

```json
{
  "Name": "MeuApp",
  "ScaffoldExceptions": [
    "Web.Blazor.Dialog",
    "Web.Blazor.Menu",
    "Domain.Validation.New",
    "Domain.Specification"
  ],
  "Contexts": { }
}
```

Depois, o scaffold ignora os artefatos listados:

```bash
forge scaffold --force --yes
# Não gera Dialog, Menu, Validation.New nem Specification
```
