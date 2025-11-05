using DataAcessLayer.Models;
using BusinessLogicLayer.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DTMS.Pages
{
    public class EditTableModel : PageModel
    {
        private readonly ITableService _tableService;

        public EditTableModel(ITableService tableService)
        {
            _tableService = tableService;
        }

        [BindProperty]
        public table Table { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var existing = _tableService.GetTableById(id);
            if (existing == null)
                return RedirectToPage("/Index");

            Table = existing;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (Table == null)
                return RedirectToPage("/Index");

            if (Table.number <= 0)
                ModelState.AddModelError("Table.number", "The number of the table needs to be positive.");

            if (!ModelState.IsValid)
                return Page();

            _tableService.UpdateTable(Table);

            return RedirectToPage("/Index");
        }
    }
}
