# 📋 Guia: Como Conectar Users e Products à Base de Dados

## 1️⃣ Criar as Tabelas na Base de Dados MySQL

Primeiro, executa o script SQL que está em `Data/database_setup.sql` na tua base de dados MySQL:

```sql
-- Tabela USERS
CREATE TABLE IF NOT EXISTS `user` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `user` VARCHAR(50) NOT NULL,
    `password` VARCHAR(50) NOT NULL,
    `role` VARCHAR(50) NOT NULL,
    INDEX `idx_user` (`user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Tabela PRODUCTS
CREATE TABLE IF NOT EXISTS `product` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(100) NOT NULL,
    `description` TEXT,
    `price` DECIMAL(10, 2) NOT NULL,
    `category` VARCHAR(50) NOT NULL,
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_category` (`category`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

**Como executar:**
1. Abre o MySQL Workbench ou o teu cliente MySQL
2. Conecta-te à base de dados `dtms`
3. Executa o script SQL acima

## 2️⃣ Ficheiros Criados

### Modelos (Models)
- ✅ `Data/Models/product.cs` - Modelo para produtos
- ✅ `Data/Models/user.cs` - Já existia, mas verifica se está correto

### Classes de Conexão (Conn)
- ✅ `Data/userconn.cs` - Classe para operações com Users (igual ao `tableconn`)
- ✅ `Data/productconn.cs` - Classe para operações com Products (igual ao `tableconn`)

## 3️⃣ Estrutura das Classes de Conexão

Cada classe (`userconn` e `productconn`) segue o mesmo padrão do `tableconn`:

### Métodos Disponíveis:

1. **GetUsers() / GetProducts()** - Obtém todos os registos
2. **GetUser(id) / GetProduct(id)** - Obtém um registo específico
3. **CreateUser() / CreateProduct()** - Cria um novo registo
4. **UpdateUser() / UpdateProduct()** - Atualiza um registo
5. **DeleteUser() / DeleteProduct()** - Elimina um registo

## 4️⃣ Como Usar no Index.cshtml.cs

Agora precisas de atualizar o `Index.cshtml.cs` para:
1. Adicionar propriedades para Users e Products
2. Adicionar métodos POST para criar Users e Products
3. Adicionar métodos para listar Users e Products
4. Adicionar métodos DELETE para Users e Products

## 5️⃣ Exemplo de Uso

### Para Users:
```csharp
var uc = new userconn();
uc.CreateUser("username", "password", "Employee");
List<user> users = uc.GetUsers();
```

### Para Products:
```csharp
var pc = new productconn();
pc.CreateProduct("Pizza", "Deliciosa pizza margherita", 12.99m, "Food");
List<product> products = pc.GetProducts();
```

## ⚠️ Notas Importantes

1. **Connection String**: Todas as classes usam a mesma connection string:
   ```csharp
   "server=localhost;port=3306;database=dtms;user=root;password=root;"
   ```

2. **Campo `user` na tabela**: A coluna chama-se `user` (palavra reservada), por isso o modelo usa `user1` como propriedade.

3. **Null Safety**: Os métodos verificam se os valores são NULL antes de ler da base de dados.

4. **Preço**: O campo `price` usa `DECIMAL(10,2)` para garantir precisão monetária.

