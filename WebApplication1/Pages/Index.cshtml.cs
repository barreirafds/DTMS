using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DTMS.Models;

namespace DTMS.Pages
{
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

        // Este método é chamado quando a página carrega
        public void OnGet()
        {
            // Não precisamos fazer nada aqui, a página vai mostrar as mesas automaticamente
        }

        // Este método é chamado quando clicamos no botão "Criar Mesa"
        public IActionResult OnPost()
        {
            // Verificar se o utilizador preencheu o número da mesa
            if (string.IsNullOrWhiteSpace(TableNumber))
            {
                // Se não preencheu, mostrar erro
                ModelState.AddModelError(nameof(TableNumber), "You need to write a number for the Table");
                return Page(); // Voltar à página mostrando o erro
            }

            // Calcular o próximo ID (número único para a mesa)
            int nextId = allTables.Count + 1;

            // Criar uma nova mesa com os dados do formulário
            Table newTable = new Table
            {
                Id = nextId,
                Number = TableNumber.Trim(), // Trim() remove espaços em branco
                Seats = TableSeats
            };

            // Adicionar a nova mesa à lista
            allTables.Add(newTable);

            // Limpar os campos do formulário
            TableNumber = string.Empty;
            TableSeats = 0;

            // Voltar à página (vai mostrar a nova mesa na lista)
            return RedirectToPage();
        }
    }
}
