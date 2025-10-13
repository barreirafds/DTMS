# Explicação linha a linha das alterações (Mesas)

Este documento descreve, linha a linha, o código adicionado para suportar a criação/listagem de mesas na página inicial.

## Models/Table.cs

``` EXPLICACAO DO CODIGO
namespace DTMS.Models
{
    public class Table
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Seats { get; set; }
    }
}
```

- `namespace DTMS.Models`: Define o namespace onde a classe `Table` vive, agrupando modelos da aplicação.
- `public class Table`: Declara a classe pública `Table`, o nosso modelo simples de mesa.
- `public int Id { get; set; }`: Identificador numérico único da mesa.
- `public string Name { get; set; } = string.Empty;`: Nome da mesa; inicia por omissão com string vazia para evitar `null`.
- `public int Seats { get; set; }`: Número de lugares da mesa.

## Pages/Index.cshtml.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DTMS.Models;

namespace DTMS.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private static readonly List<Table> _tables = new();

        [BindProperty]
        public string NewTableName { get; set; } = string.Empty;

        [BindProperty]
        public int NewTableSeats { get; set; }

        public IReadOnlyList<Table> Tables => _tables;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPostAdd()
        {
            if (string.IsNullOrWhiteSpace(NewTableName))
            {
                ModelState.AddModelError(nameof(NewTableName), "Nome é obrigatório");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var nextId = _tables.Count == 0 ? 1 : _tables.Max(t => t.Id) + 1;
            _tables.Add(new Table
            {
                Id = nextId,
                Name = NewTableName.Trim(),
                Seats = NewTableSeats
            });

            // Clear fields after add
            NewTableName = string.Empty;
            NewTableSeats = 0;

            return RedirectToPage();
        }
    }
}
```

- `using Microsoft.AspNetCore.Mvc;`: Importa atributos e tipos MVC (ex.: `IActionResult`, `BindProperty`).
- `using Microsoft.AspNetCore.Mvc.RazorPages;`: Importa o suporte a Razor Pages (`PageModel`).
- `using DTMS.Models;`: Importa o nosso modelo `Table`.
- `namespace DTMS.Pages`: Namespace das páginas Razor.
- `public class IndexModel : PageModel`: PageModel da página Index; contém handlers e dados para a view.
- `private readonly ILogger<IndexModel> _logger;`: Logger opcional para registos.
- `private static readonly List<Table> _tables = new();`: Lista estática em memória onde guardamos as mesas durante a execução.
- `[BindProperty] public string NewTableName { get; set; } = string.Empty;`: Propriedade ligada ao formulário para nome da mesa.
- `[BindProperty] public int NewTableSeats { get; set; }`: Propriedade ligada ao formulário para número de lugares.
- `public IReadOnlyList<Table> Tables => _tables;`: Exposição somente leitura da lista para a view.
- `public IndexModel(ILogger<IndexModel> logger) { _logger = logger; }`: Injeta e guarda o logger.
- `public void OnGet() { }`: Handler GET padrão (sem lógica específica aqui).
- `public IActionResult OnPostAdd()`: Handler POST para o formulário com handler "Add".
- `if (string.IsNullOrWhiteSpace(NewTableName)) { ... }`: Validação simples: nome obrigatório.
- `ModelState.AddModelError(...)`: Adiciona erro de validação para o campo.
- `if (!ModelState.IsValid) { return Page(); }`: Se houver erros, re-renderiza a página mostrando-os.
- `var nextId = ...`: Calcula o próximo Id com base na lista atual.
- `_tables.Add(new Table { ... })`: Cria e adiciona a mesa à lista em memória.
- `NewTableName = string.Empty; NewTableSeats = 0;`: Limpa os campos após adicionar.
- `return RedirectToPage();`: Redireciona para GET para evitar repost do formulário e atualizar a lista.

## Pages/Index.cshtml

```html
@page
@model IndexModel
@{
    ViewData["Title"] = "Mesas";
}

<h1>Mesas</h1>

<form method="post">
    <input type="hidden" name="handler" value="Add" />
    <div>
        <label for="NewTableName">Nome da mesa</label>
        <input id="NewTableName" name="NewTableName" value="@Model.NewTableName" />
        <span asp-validation-for="NewTableName"></span>
    </div>
    <div>
        <label for="NewTableSeats">Lugares</label>
        <input id="NewTableSeats" name="NewTableSeats" type="number" value="@Model.NewTableSeats" />
    </div>
    <button type="submit">Adicionar</button>
</form>

<h2>Lista</h2>
<ul>
@foreach (var t in Model.Tables)
{
    <li>#@t.Id - @t.Name (@t.Seats lugares)</li>
}
</ul>
```

- `@page`: Indica que este ficheiro é uma Razor Page acessível diretamente.
- `@model IndexModel`: Define o PageModel associado à view.
- `ViewData["Title"] = "Mesas";`: Define o título da página.
- `<h1>Mesas</h1>`: Cabeçalho simples.
- `<form method="post">`: Formulário que envia via POST.
- `<input type="hidden" name="handler" value="Add" />`: Define que o POST deve invocar `OnPostAdd` no PageModel.
- `<label ...> / <input ... name="NewTableName" ...>`: Campo para o nome da mesa ligado à propriedade `NewTableName`.
- `<span asp-validation-for="NewTableName"></span>`: Local para mostrar mensagens de validação do campo de nome.
- `<input ... name="NewTableSeats" type="number" ...>`: Campo numérico para os lugares, ligado a `NewTableSeats`.
- `<button type="submit">Adicionar</button>`: Botão de submissão do formulário.
- `<h2>Lista</h2>`: Cabeçalho da listagem.
- `@foreach (var t in Model.Tables) { ... }`: Itera as mesas atuais na lista em memória.
- `<li>#@t.Id - @t.Name (@t.Seats lugares)</li>`: Mostra cada mesa com Id, nome e lugares.

---
