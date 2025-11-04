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

        // Users Properties
        [BindProperty]
        public string UserName { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string UserRole { get; set; } = string.Empty;

        public List<user> UsersList { get; private set; } = new();

        // Products Properties
        [BindProperty]
        public string ProductName { get; set; } = string.Empty;

        [BindProperty]
        public string? ProductDescription { get; set; }

        [BindProperty]
        public decimal ProductPrice { get; set; }

        [BindProperty]
        public string ProductCategory { get; set; } = string.Empty;

        public List<product> ProductsList { get; private set; } = new();

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
            UsersList = new userconn().GetUsers();
            ProductsList = new productconn().GetProducts();
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

        // Users CRUD
        public async Task<IActionResult> OnPostCreateUser()
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(UserRole))
            {
                ModelState.AddModelError("", "All fields are required.");
                TablesList = new tableconn().GetTables();
                UsersList = new userconn().GetUsers();
                ProductsList = new productconn().GetProducts();
                return Page();
            }

            var uc = new userconn();
            uc.CreateUser(UserName, Password, UserRole);

            return RedirectToPage();
        }

        public IActionResult OnPostDeleteUser(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToPage();
            }

            var uc = new userconn();
            uc.DeleteUser(id.Value);
            return RedirectToPage();
        }

        // Products CRUD 
        public async Task<IActionResult> OnPostCreateProduct()
        {
            if (string.IsNullOrWhiteSpace(ProductName) || string.IsNullOrWhiteSpace(ProductCategory))
            {
                ModelState.AddModelError("", "Product name and category are required.");
                TablesList = new tableconn().GetTables();
                UsersList = new userconn().GetUsers();
                ProductsList = new productconn().GetProducts();
                return Page();
            }

            if (ProductPrice <= 0)
            {
                ModelState.AddModelError("", "Product price must be greater than 0.");
                TablesList = new tableconn().GetTables();
                UsersList = new userconn().GetUsers();
                ProductsList = new productconn().GetProducts();
                return Page();
            }

            var pc = new productconn();
            pc.CreateProduct(ProductName, ProductDescription, ProductPrice, ProductCategory);

            return RedirectToPage();
        }

        public IActionResult OnPostDeleteProduct(int id)
        {
            var pc = new productconn();
            pc.DeleteProduct(id);
            return RedirectToPage();
        }

    }
}
