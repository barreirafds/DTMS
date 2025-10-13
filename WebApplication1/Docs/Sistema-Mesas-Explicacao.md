# 🍽️ Sistema de Gestão de Mesas - Explicação Completa

Este documento explica como funciona o sistema de gestão de mesas, linha por linha, para que possas entender completamente como tudo funciona.

## 📁 Estrutura do Projeto

```
WebApplication1/
├── Models/
│   └── Table.cs          # Modelo que representa uma mesa
├── Pages/
│   ├── Index.cshtml      # Página HTML (interface visual)
│   └── Index.cshtml.cs   # Código C# (lógica da página)
└── Docs/
    └── Sistema-Mesas-Explicacao.md  # Este documento
```

---

## 🏗️ 1. Modelo de Dados (Table.cs)

O modelo `Table` é como um "molde" que define como é uma mesa no nosso sistema.

```csharp
namespace DTMS.Models
{
    public class Table
    {
        public int Id { get; set; }           // Número único da mesa (1, 2, 3...)
        public string Number { get; set; }    // Nome/número da mesa ("Mesa 5", "A1"...)
        public int Seats { get; set; }        // Quantos lugares tem a mesa
    }
}
```

**Explicação:**
- `Id`: É como o "bilhete de identidade" da mesa - cada mesa tem um número único
- `Number`: É o nome que aparece na mesa (ex: "5", "VIP1", "Janela")
- `Seats`: Quantas pessoas podem sentar-se nesta mesa

---

## 🎨 2. Interface Visual (Index.cshtml)

Esta é a página que o utilizador vê no browser. Tem duas partes principais:

### A) Formulário para Criar Mesa
```html
<form method="post">
    <h3>Criar Nova Mesa</h3>
    
    <div>
        <label for="TableNumber">Número da Mesa:</label><br>
        <input id="TableNumber" name="TableNumber" value="@Model.TableNumber" placeholder="Ex: 5" />
        <span asp-validation-for="TableNumber" style="color: red;"></span>
    </div>
    
    <div>
        <label for="TableSeats">Número de Lugares:</label><br>
        <input id="TableSeats" name="TableSeats" type="number" value="@Model.TableSeats" placeholder="Ex: 4" />
    </div>
    
    <button type="submit">➕ Criar Mesa</button>
</form>
```

**Como funciona:**
- `<form method="post">`: Quando clicas no botão, os dados vão para o servidor
- `name="TableNumber"`: Liga este campo à propriedade `TableNumber` no código C#
- `@Model.TableNumber`: Mostra o valor atual (vazio quando crias, preenchido se houver erro)
- `<span asp-validation-for="TableNumber">`: Mostra mensagens de erro em vermelho

### B) Lista de Mesas Existentes
```html
<h2>📋 Lista de Mesas</h2>
@if (Model.TablesList.Any())
{
    <ul>
        @foreach (var table in Model.TablesList)
        {
            <li>
                <strong>Mesa #@table.Number</strong> - @table.Seats lugares
            </li>
        }
    </ul>
}
```

**Como funciona:**
- `@if (Model.TablesList.Any())`: Se existirem mesas, mostra a lista
- `@foreach (var table in Model.TablesList)`: Para cada mesa na lista...
- `@table.Number` e `@table.Seats`: Mostra os dados da mesa

---

## ⚙️ 3. Lógica da Página (Index.cshtml.cs)

Este é o "cérebro" da aplicação. Vamos analisar cada parte:

### A) Declaração de Variáveis
```csharp
public class IndexModel : PageModel
{
    // Lista onde guardamos todas as mesas (fica na memória enquanto a aplicação está a correr)
    private static readonly List<Table> allTables = new()
    {
        new Table { Id = 1, Number = "1", Seats = 4 } // Mesa de exemplo
    };

    // Propriedades que vão receber os dados do formulário
    [BindProperty]
    public string TableNumber { get; set; } = string.Empty;

    [BindProperty]
    public int TableSeats { get; set; }

    // Propriedade que a página HTML vai usar para mostrar a lista de mesas
    public List<Table> TablesList => allTables;
}
```

**Explicação:**
- `allTables`: Lista que guarda todas as mesas (como uma gaveta virtual)
- `[BindProperty]`: Liga os campos do formulário HTML a estas propriedades
- `TablesList`: Dá acesso à lista para o HTML mostrar

### B) Método OnGet() - Quando a Página Carrega
```csharp
public void OnGet()
{
    // Não precisamos fazer nada aqui, a página vai mostrar as mesas automaticamente
}
```

**Quando é chamado:**
- Quando abres a página pela primeira vez
- Quando voltas à página após criar uma mesa

**O que faz:**
- Nada! A página mostra automaticamente as mesas que estão na lista `allTables`

### C) Método OnPost() - Quando Clicas "Criar Mesa"
```csharp
public IActionResult OnPost()
{
    // 1. Verificar se o utilizador preencheu o número da mesa
    if (string.IsNullOrWhiteSpace(TableNumber))
    {
        // Se não preencheu, mostrar erro
        ModelState.AddModelError(nameof(TableNumber), "Tem de escrever um número para a mesa");
        return Page(); // Voltar à página mostrando o erro
    }

    // 2. Calcular o próximo ID (número único para a mesa)
    int nextId = allTables.Count + 1;

    // 3. Criar uma nova mesa com os dados do formulário
    Table newTable = new Table
    {
        Id = nextId,
        Number = TableNumber.Trim(), // Trim() remove espaços em branco
        Seats = TableSeats
    };

    // 4. Adicionar a nova mesa à lista
    allTables.Add(newTable);

    // 5. Limpar os campos do formulário
    TableNumber = string.Empty;
    TableSeats = 0;

    // 6. Voltar à página (vai mostrar a nova mesa na lista)
    return RedirectToPage();
}
```

**Passo a passo:**
1. **Validação**: Verifica se escreveste um número para a mesa
2. **ID único**: Calcula o próximo número disponível (1, 2, 3...)
3. **Criar mesa**: Cria um novo objeto `Table` com os teus dados
4. **Guardar**: Adiciona a mesa à lista `allTables`
5. **Limpar**: Apaga os campos do formulário
6. **Atualizar**: Volta à página para mostrares a nova mesa

---

## 🔄 4. Fluxo Completo - O Que Acontece Quando Crias uma Mesa

### Passo 1: Utilizador preenche o formulário
```
Número da Mesa: "5"
Número de Lugares: 4
```

### Passo 2: Clica "Criar Mesa"
- O browser envia os dados para o servidor
- O método `OnPost()` é chamado automaticamente

### Passo 3: Validação
```csharp
if (string.IsNullOrWhiteSpace(TableNumber)) // "5" não está vazio, continua
```

### Passo 4: Criar nova mesa
```csharp
int nextId = allTables.Count + 1; // Se há 1 mesa, nextId = 2

Table newTable = new Table
{
    Id = 2,                    // Próximo ID disponível
    Number = "5",              // O que escreveste
    Seats = 4                  // O que escreveste
};
```

### Passo 5: Guardar na lista
```csharp
allTables.Add(newTable); // Lista agora tem 2 mesas
```

### Passo 6: Limpar e voltar
```csharp
TableNumber = "";  // Campo fica vazio
TableSeats = 0;    // Campo fica vazio
return RedirectToPage(); // Volta à página
```

### Passo 7: Página atualiza
- `OnGet()` é chamado (mas não faz nada)
- HTML mostra a lista atualizada com a nova mesa

---

## 🎯 5. Conceitos Importantes

### A) Model Binding
```csharp
[BindProperty]
public string TableNumber { get; set; }
```
- Liga automaticamente o campo HTML `name="TableNumber"` a esta propriedade
- Quando submetes o formulário, o valor vai automaticamente para `TableNumber`

### B) Static List
```csharp
private static readonly List<Table> allTables = new();
```
- `static`: A lista existe durante toda a vida da aplicação
- `readonly`: Não podes trocar a lista, mas podes adicionar/remover itens
- **Problema**: Se reiniciares a aplicação, as mesas desaparecem (só ficam na memória)

### C) Razor Syntax
```html
@Model.TableNumber        <!-- Mostra o valor da propriedade -->
@foreach (var table in Model.TablesList)  <!-- Loop para cada mesa -->
@table.Number            <!-- Mostra o número da mesa atual -->
```

### D) Validation
```csharp
ModelState.AddModelError(nameof(TableNumber), "Mensagem de erro");
```
- Adiciona um erro ao modelo
- O HTML mostra automaticamente a mensagem em vermelho

---

## 🚀 6. Como Testar o Sistema

1. **Abrir a aplicação**: Vai para `https://localhost:7003`
2. **Ver mesa de exemplo**: Deve aparecer "Mesa #1 - 4 lugares"
3. **Criar nova mesa**:
   - Escreve "5" no campo "Número da Mesa"
   - Escreve "6" no campo "Número de Lugares"
   - Clica "Criar Mesa"
4. **Verificar**: Deve aparecer "Mesa #5 - 6 lugares" na lista
5. **Testar validação**: Deixa o campo vazio e clica "Criar Mesa" - deve aparecer erro

---

## 🔧 7. Possíveis Melhorias

### A) Base de Dados
- Atualmente as mesas ficam só na memória
- Podias usar uma base de dados para guardar permanentemente

### B) Editar/Eliminar Mesas
- Adicionar botões para editar ou eliminar mesas existentes

### C) Validações Adicionais
- Verificar se já existe uma mesa com o mesmo número
- Validar se o número de lugares é positivo

### D) Interface Melhorada
- Usar CSS mais avançado
- Adicionar confirmações antes de eliminar

---

## 📚 8. Resumo dos Ficheiros

| Ficheiro | Função | O que contém |
|----------|--------|--------------|
| `Table.cs` | Modelo | Define como é uma mesa (Id, Number, Seats) |
| `Index.cshtml` | Interface | HTML que o utilizador vê |
| `Index.cshtml.cs` | Lógica | Código C# que processa os dados |
| `Program.cs` | Configuração | Configura a aplicação web |

---

## 🎓 9. Conceitos de Programação Aplicados

- **Classes e Objetos**: `Table` é uma classe, cada mesa é um objeto
- **Collections**: `List<Table>` para guardar múltiplas mesas
- **Model Binding**: Ligação automática entre HTML e C#
- **Validation**: Verificação de dados antes de processar
- **MVC Pattern**: Model (Table), View (HTML), Controller (IndexModel)
- **Static Variables**: Variáveis que persistem durante a vida da aplicação

---

Este sistema é um exemplo perfeito de uma aplicação web simples mas funcional, que demonstra os conceitos fundamentais de desenvolvimento web com ASP.NET Core! 🚀
