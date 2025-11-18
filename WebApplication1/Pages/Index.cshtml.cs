using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using DataAcessLayer.Repositories;
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

        public List<TableDTO> TablesList { get; private set; } = new();

        // Users Properties
        [BindProperty]
        public string UserName { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string UserRole { get; set; } = string.Empty;

        public List<UserDTO> UsersList { get; private set; } = new();

        // Products Properties
        [BindProperty]
        public string ProductName { get; set; } = string.Empty;

        [BindProperty]
        public string? ProductDescription { get; set; }

        [BindProperty]
        public decimal ProductPrice { get; set; }

        [BindProperty]
        public string ProductCategory { get; set; } = string.Empty;

        public List<ProductDTO> ProductsList { get; private set; } = new();

        public string GetStatusBadgeStyle(string? status)
        {
            return _tableService.GetStatusBadgeStyle(status ?? string.Empty);
        }

        public void OnGet()
        {
            TablesList = new TableService(new TableRepository()).GetAllTables();
            //TablesList = _tableService.GetAllTables();
            UsersList = _userService.GetAllUsers();
            ProductsList = _productService.GetAllProducts();
        }

        public IActionResult OnPost()
        {
            var createTableDto = new CreateTableDTO
            {
                TableNumber = TableNumber,
                Seats = TableSeats,
                Status = TableStatus
            };

            var result = _tableService.CreateTable(createTableDto);
            
            if (!result.IsValid)
            {
                ModelState.AddModelError(result.FieldName ?? "", result.ErrorMessage ?? "");
                OnGet();
                return Page();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            _tableService.DeleteTable(id);
            return RedirectToPage();
        }

        // Users CRUD
        public IActionResult OnPostCreateUser()
        {
            var createUserDto = new CreateUserDTO
            {
                Username = UserName,
                Password = Password,
                Role = UserRole
            };

            var result = _userService.CreateUser(createUserDto);
            
            if (!result.IsValid)
            {
                ModelState.AddModelError(result.FieldName ?? "", result.ErrorMessage ?? "");
                OnGet();
                return Page();
            }

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
        public IActionResult OnPostCreateProduct()
        {
            var createProductDto = new CreateProductDTO
            {
                name = ProductName,
                description = ProductDescription,
                price = ProductPrice,
                category = ProductCategory
            };

            var result = _productService.CreateProduct(createProductDto);
            
            if (!result.IsValid)
            {
                ModelState.AddModelError(result.FieldName ?? "", result.ErrorMessage ?? "");
                OnGet();
                return Page();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDeleteProduct(int id)
        {
            _productService.DeleteProduct(id);
            return RedirectToPage();
        }
    }
}
