using DTMS.Data;
using DTMS.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace DTMS.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty, Required(ErrorMessage = "Number of the table is required")]
        public string TableNumber { get; set; } = string.Empty;

        [BindProperty]
        public int TableSeats { get; set; }

        public List<table> TablesList { get; private set; } = new();

        public async Task OnGet()
        {
            TablesList = new tableconn().GetTables();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!int.TryParse(TableNumber, out var number))
            {
                ModelState.AddModelError(nameof(TableNumber), "Table Number needs to be a number.");
            }

            if (!ModelState.IsValid)
            {
                TablesList = new tableconn().GetTables();
                return Page();
            }

            var tc = new tableconn();
            tc.CreateTable(number, TableSeats);

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var tc = new tableconn();
            tc.DeleteTable(id);
            return RedirectToPage();
        }

    }
}
