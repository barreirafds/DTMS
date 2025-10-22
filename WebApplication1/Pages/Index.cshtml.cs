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

        [BindProperty]
        public string TableStatus { get; set; } = "Available";

        public List<table> TablesList { get; private set; } = new();

        public string GetStatusBadgeStyle(string status)
        {
            var s = (status ?? "Available").ToLowerInvariant();
            string bg = "#e2e3e5"; 
            string fg = "#383d41";
            
            if (s == "available") { bg = "#d4edda"; fg = "#155724"; }
            else if (s == "occupied") { bg = "#f8d7da"; fg = "#721c24"; }
            else if (s == "reserved") { bg = "#fff3cd"; fg = "#856404"; }
            else if (s == "outofservice") { bg = "#e2e3e5"; fg = "#383d41"; }
            
            return $"display:inline-block; margin-left:8px; padding:2px 8px; border-radius:12px; font-size:12px; background:{bg}; color:{fg};";
        }

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
            tc.CreateTable(number, TableSeats, TableStatus);

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
