# Arquitetura do Projeto DTMS

## Visão Geral

Este projeto segue uma arquitetura em camadas (Layered Architecture) que separa as responsabilidades em três camadas principais:

1. **DataAccessLayer** - Camada de Acesso a Dados
2. **BusinessLogicLayer** - Camada de Lógica de Negócio
3. **WebApplication1** - Camada de Apresentação (UI)

---

## Estrutura das Camadas

### 1. DataAccessLayer

**Localização:** `DataAccessLayer/`

**Responsabilidades:**
- Acesso direto à base de dados MySQL
- Operações CRUD (Create, Read, Update, Delete)
- Gestão de conexões com a base de dados
- Mapeamento de dados da base de dados para modelos

**Estrutura:**
```
DataAccessLayer/
├── Models/          # Modelos de dados (entidades)
│   ├── product.cs
│   ├── table.cs
│   └── user.cs
├── productconn.cs    # Classe de conexão para produtos
├── tableconn.cs      # Classe de conexão para mesas
└── userconn.cs       # Classe de conexão para utilizadores
```

**Modelos:**
- `product` - Representa um produto no sistema
- `table` - Representa uma mesa no restaurante
- `user` - Representa um utilizador do sistema

**Classes de Conexão:**
- `productconn` - Métodos para operações CRUD de produtos
- `tableconn` - Métodos para operações CRUD de mesas
- `userconn` - Métodos para operações CRUD de utilizadores

**Nota:** Esta camada contém apenas lógica de acesso a dados. Não contém validações de negócio ou regras de aplicação.

---

### 2. BusinessLogicLayer

**Localização:** `BusinessLogicLayer/`

**Responsabilidades:**
- Implementação de toda a lógica de negócio
- Validações de dados e regras de negócio
- Transformação entre modelos de dados e DTOs
- Orquestração de operações complexas
- Fornecimento de interfaces para a camada de apresentação

**Estrutura:**
```
BusinessLogicLayer/
├── Abstractions/     # Interfaces dos serviços
│   ├── IAuthService.cs
│   ├── IProductService.cs
│   ├── ITableService.cs
│   └── IUserService.cs
├── Services/         # Implementações dos serviços
│   ├── AuthService.cs
│   ├── ProductService.cs
│   ├── TableService.cs
│   └── UserService.cs
└── DTOs/             # Data Transfer Objects
    ├── ProductDTO.cs
    ├── TableDTO.cs
    ├── UserDTO.cs
    └── ValidationResult.cs
```

#### Abstractions (Interfaces)

As interfaces definem os contratos que os serviços devem implementar:

- **`IAuthService`** - Autenticação e registo de utilizadores
  - `ValidateCredentials(LoginDTO)` - Valida credenciais de login
  - `RegisterUser(RegisterDTO)` - Regista novo utilizador

- **`IProductService`** - Gestão de produtos
  - `GetAllProducts()` - Obtém todos os produtos
  - `GetProductById(int)` - Obtém produto por ID
  - `CreateProduct(CreateProductDTO)` - Cria novo produto
  - `UpdateProduct(ProductDTO)` - Atualiza produto existente
  - `DeleteProduct(int)` - Elimina produto

- **`ITableService`** - Gestão de mesas
  - `GetAllTables()` - Obtém todas as mesas
  - `GetTableById(int)` - Obtém mesa por ID
  - `CreateTable(CreateTableDTO)` - Cria nova mesa
  - `UpdateTable(UpdateTableDTO)` - Atualiza mesa existente
  - `DeleteTable(int)` - Elimina mesa
  - `GetStatusBadgeStyle(string)` - Retorna estilo CSS para badge de status

- **`IUserService`** - Gestão de utilizadores
  - `GetAllUsers()` - Obtém todos os utilizadores
  - `GetUserById(int)` - Obtém utilizador por ID
  - `CreateUser(CreateUserDTO)` - Cria novo utilizador
  - `UpdateUser(UserDTO)` - Atualiza utilizador existente
  - `DeleteUser(int)` - Elimina utilizador

#### Services (Implementações)

Os serviços implementam a lógica de negócio:

- **`AuthService`** - Implementa autenticação e registo
  - Valida credenciais contra base de dados
  - Valida dados de registo (campos obrigatórios, passwords coincidem, username único)
  - Cria novos utilizadores

- **`ProductService`** - Implementa gestão de produtos
  - Valida dados de produtos (nome, categoria obrigatórios, preço > 0)
  - Transforma entre modelos de dados e DTOs
  - Chama métodos da DataAccessLayer

- **`TableService`** - Implementa gestão de mesas
  - Valida dados de mesas (número válido, número > 0, lugares > 0, status obrigatório)
  - Gera estilos CSS para badges de status
  - Transforma entre modelos de dados e DTOs

- **`UserService`** - Implementa gestão de utilizadores
  - Valida dados de utilizadores (campos obrigatórios, username único)
  - Transforma entre modelos de dados e DTOs

#### DTOs (Data Transfer Objects)

Os DTOs são objetos que transferem dados entre camadas sem expor os modelos internos:

- **`ProductDTO`** - DTO para produtos
  - `ProductDTO` - DTO completo para leitura
  - `CreateProductDTO` - DTO para criação de produtos

- **`TableDTO`** - DTOs para mesas
  - `TableDTO` - DTO completo para leitura
  - `CreateTableDTO` - DTO para criação de mesas
  - `UpdateTableDTO` - DTO para atualização de mesas

- **`UserDTO`** - DTOs para utilizadores
  - `UserDTO` - DTO completo para leitura
  - `CreateUserDTO` - DTO para criação de utilizadores
  - `LoginDTO` - DTO para login
  - `RegisterDTO` - DTO para registo

- **`ValidationResult`** - Resultado de validação
  - `IsValid` - Indica se a validação passou
  - `ErrorMessage` - Mensagem de erro (se houver)
  - `FieldName` - Nome do campo com erro (se aplicável)
  - Métodos estáticos: `Success()` e `Failure()`

**Validações Implementadas:**

- **Mesas:**
  - Número da mesa deve ser um número válido
  - Número da mesa deve ser maior que 0
  - Número de lugares deve ser maior que 0
  - Status é obrigatório

- **Produtos:**
  - Nome e categoria são obrigatórios
  - Preço deve ser maior que 0

- **Utilizadores:**
  - Todos os campos são obrigatórios
  - Username deve ser único

- **Autenticação:**
  - Username e password são obrigatórios
  - Passwords devem coincidir no registo
  - Username deve ser único no registo

---

### 3. WebApplication1 (UI Layer)

**Localização:** `WebApplication1/`

**Responsabilidades:**
- Apresentação de dados ao utilizador
- Receção de input do utilizador
- Navegação entre páginas
- Renderização de HTML/CSS
- **NÃO contém lógica de negócio** - apenas delega para os serviços

**Estrutura:**
```
WebApplication1/
├── Pages/            # Páginas Razor Pages
│   ├── Index.cshtml.cs      # Modelo da página principal
│   ├── Index.cshtml          # View da página principal
│   ├── Login.cshtml.cs       # Modelo da página de login
│   ├── Login.cshtml          # View da página de login
│   ├── Register.cshtml.cs     # Modelo da página de registo
│   ├── Register.cshtml       # View da página de registo
│   └── EditTable.cshtml.cs   # Modelo da página de edição
│   └── EditTable.cshtml      # View da página de edição
└── Program.cs        # Configuração da aplicação e DI
```

**Páginas:**

- **`Index`** - Dashboard administrativo
  - Lista mesas, utilizadores e produtos
  - Permite criar novos registos
  - Permite eliminar registos
  - Usa DTOs para exibir dados
  - Delega validações para os serviços

- **`Login`** - Página de autenticação
  - Recebe username e password
  - Delega validação para `AuthService`
  - Redireciona para Index em caso de sucesso

- **`Register`** - Página de registo
  - Recebe dados de registo
  - Delega validação e criação para `AuthService`
  - Redireciona para Login em caso de sucesso

- **`EditTable`** - Página de edição de mesa
  - Permite editar dados de uma mesa
  - Delega validação e atualização para `TableService`

**Program.cs:**
- Configura a aplicação ASP.NET Core
- Regista serviços no container de Dependency Injection:
  - `ITableService` → `TableService`
  - `IUserService` → `UserService`
  - `IProductService` → `ProductService`
  - `IAuthService` → `AuthService`

---

## Fluxo de Dados

### Exemplo: Criar uma Mesa

1. **UI Layer** (`Index.cshtml.cs`)
   - Utilizador preenche formulário e submete
   - Página recebe dados e cria `CreateTableDTO`
   - Chama `_tableService.CreateTable(createTableDto)`

2. **BusinessLogicLayer** (`TableService`)
   - Recebe `CreateTableDTO`
   - Valida dados (número válido, > 0, lugares > 0, status obrigatório)
   - Se válido, cria objeto `table` e chama `_tableConn.CreateTable()`
   - Retorna `ValidationResult`

3. **DataAccessLayer** (`tableconn`)
   - Recebe dados validados
   - Abre conexão com MySQL
   - Executa INSERT na base de dados
   - Fecha conexão

4. **Resposta**
   - Se sucesso: UI redireciona para página principal
   - Se erro: UI exibe mensagem de erro ao utilizador

---

## Princípios Aplicados

### 1. Separation of Concerns (Separação de Responsabilidades)
- Cada camada tem uma responsabilidade específica
- A UI não conhece a estrutura da base de dados
- A BusinessLogicLayer não conhece detalhes de apresentação

### 2. Dependency Inversion Principle (Princípio da Inversão de Dependência)
- A UI depende de interfaces (`IAuthService`, `ITableService`, etc.)
- Não depende de implementações concretas
- Facilita testes e manutenção

### 3. Single Responsibility Principle (Princípio da Responsabilidade Única)
- Cada serviço tem uma responsabilidade específica
- Cada DTO serve um propósito específico
- Cada classe de conexão gere uma entidade específica

### 4. Data Transfer Objects (DTOs)
- Isolam a camada de apresentação dos modelos internos
- Permitem evolução independente das camadas
- Facilitam validação e transformação de dados

---

## Vantagens desta Arquitetura

1. **Manutenibilidade**
   - Código organizado e fácil de localizar
   - Alterações em uma camada não afetam outras diretamente

2. **Testabilidade**
   - Serviços podem ser testados isoladamente
   - Interfaces permitem criação de mocks

3. **Escalabilidade**
   - Fácil adicionar novas funcionalidades
   - Fácil modificar validações ou regras de negócio

4. **Reutilização**
   - Serviços podem ser reutilizados por diferentes interfaces (Web, API, etc.)

5. **Clareza**
   - Cada camada tem propósito claro
   - Fácil para novos desenvolvedores entenderem

---

## Como Adicionar Nova Funcionalidade

### Exemplo: Adicionar gestão de Pedidos

1. **DataAccessLayer**
   - Criar `Models/order.cs`
   - Criar `orderconn.cs` com métodos CRUD

2. **BusinessLogicLayer**
   - Criar `DTOs/OrderDTO.cs` com DTOs necessários
   - Criar `Abstractions/IOrderService.cs` com interface
   - Criar `Services/OrderService.cs` com lógica de negócio e validações

3. **WebApplication1**
   - Registar `IOrderService` em `Program.cs`
   - Criar página `Orders.cshtml` e `Orders.cshtml.cs`
   - Usar `IOrderService` na página

---

## Notas Importantes

- **Validações:** Toda a lógica de validação está na BusinessLogicLayer, não na UI
- **DTOs:** A UI nunca usa diretamente os modelos da DataAccessLayer
- **Serviços:** Todos os serviços retornam `ValidationResult` para operações que podem falhar
- **Dependency Injection:** Todos os serviços são registados em `Program.cs` e injetados via construtor

---

## Tecnologias Utilizadas

- **.NET 8.0** - Framework principal
- **ASP.NET Core Razor Pages** - Framework web
- **MySQL** - Base de dados
- **MySql.Data.MySqlClient** - Driver MySQL para .NET

---

**Data de Criação:** 2025-01-12  
**Última Atualização:** 2025-01-12

