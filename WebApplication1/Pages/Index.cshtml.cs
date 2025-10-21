using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DTMS.Data.Models;

namespace DTMS.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        public IndexModel(AppDbContext db) => _db = db;

        [BindProperty]  
        public string TableNumber { get; set; } = string.Empty;

        [BindProperty]
        public int TableSeats { get; set; }

        public List<table> TablesList { get; private set; } = new();

        public async Task OnGet()
        {
            TablesList = await _db.tables.OrderBy(t => t.id).ToListAsync();
        }

        public async Task<IActionResult> OnPost()
        {
            if (string.IsNullOrWhiteSpace(TableNumber))
            {
                ModelState.AddModelError(nameof(TableNumber), "You need to write a number for the Table");
                return Page();
            }

            int nextId = (_db.tables.Max(t => (int?)t.id) ?? 0) + 1;

            var newTable = new table
            {
                id = nextId, // Assuming 'id' is auto-generated
                number = int.Parse(TableNumber),
                seats = TableSeats
            };

            await _db.tables.AddAsync(newTable);
            await _db.SaveChangesAsync();

            // Limpa o formulário (opcional)
            TableNumber = string.Empty;
            TableSeats = 0;

            return RedirectToPage();
        }
    }
}
