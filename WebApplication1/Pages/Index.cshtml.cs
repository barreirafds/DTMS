using DataAcessLayer.Models;
using BusinessLogicLayer.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;


namespace DTMS.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ITableService _tableService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public IndexModel(ITableService tableService, IUserService userService, IProductService productService)
        {
            _tableService = tableService;
            _userService = userService;
            _productService = productService;
        }

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
            return _tableService.GetStatusBadgeStyle(status);
        }

        public async Task OnGet()
        {
            TablesList = _tableService.GetAllTables();
            UsersList = _userService.GetAllUsers();
            ProductsList = _productService.GetAllProducts();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!int.TryParse(TableNumber, out var number))
            {
                ModelState.AddModelError(nameof(TableNumber), "Table Number needs to be a number.");
            }

            if (!ModelState.IsValid)
            {
                TablesList = _tableService.GetAllTables();
                return Page();
            }

            _tableService.CreateTable(number, TableSeats, TableStatus);

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            _tableService.DeleteTable(id);
            return RedirectToPage();
        }

        // Users CRUD
        public async Task<IActionResult> OnPostCreateUser()
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(UserRole))
            {
                ModelState.AddModelError("", "All fields are required.");
                TablesList = _tableService.GetAllTables();
                UsersList = _userService.GetAllUsers();
                ProductsList = _productService.GetAllProducts();
                return Page();
            }

            _userService.CreateUser(UserName, Password, UserRole);

            return RedirectToPage();
        }

        public IActionResult OnPostDeleteUser(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToPage();
            }

            _userService.DeleteUser(id.Value);
            return RedirectToPage();
        }

        // Products CRUD 
        public async Task<IActionResult> OnPostCreateProduct()
        {
            if (string.IsNullOrWhiteSpace(ProductName) || string.IsNullOrWhiteSpace(ProductCategory))
            {
                ModelState.AddModelError("", "Product name and category are required.");
                TablesList = _tableService.GetAllTables();
                UsersList = _userService.GetAllUsers();
                ProductsList = _productService.GetAllProducts();
                return Page();
            }

            if (ProductPrice <= 0)
            {
                ModelState.AddModelError("", "Product price must be greater than 0.");
                TablesList = _tableService.GetAllTables();
                UsersList = _userService.GetAllUsers();
                ProductsList = _productService.GetAllProducts();
                return Page();
            }

            _productService.CreateProduct(ProductName, ProductDescription, ProductPrice, ProductCategory);

            return RedirectToPage();
        }

        public IActionResult OnPostDeleteProduct(int id)
        {
            _productService.DeleteProduct(id);
            return RedirectToPage();
        }

    }
}
