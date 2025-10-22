using DTMS.Data;
using DTMS.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DTMS.Pages
{
    public class EditTableModel : PageModel
    {
        [BindProperty]
        public table Table { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var tc = new tableconn();
            var existing = tc.GetTable(id);
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

            var tc = new tableconn();
            tc.UpdateTable(Table);

            return RedirectToPage("/Index");
        }
    }
}
