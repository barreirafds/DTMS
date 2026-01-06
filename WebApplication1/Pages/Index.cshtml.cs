using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using DataAcessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;

namespace DTMS.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ITableService _tableService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IUserRepository _userRepository;

        public IndexModel(ITableService tableService, IUserService userService, IProductService productService, IOrderService orderService, IUserRepository userRepository)
        {
            _tableService = tableService;
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
            _userRepository = userRepository;
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

        // Helper method to get or create user from Auth0 claims
        private int GetOrCreateUserIdFromAuth0()
        {
            // Try to get the user identifier from Auth0 claims
            // Auth0 uses "sub" claim for user ID, or we can use email/name
            var subClaim = User.FindFirst("sub")?.Value;
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            var nameClaim = User.FindFirst(ClaimTypes.Name)?.Value ?? User.FindFirst("name")?.Value;
            
            // Use email if available, otherwise use name, otherwise use sub
            var username = emailClaim ?? nameClaim ?? subClaim ?? "auth0_user";
            
            // Try to find existing user by username
            var existingUser = _userRepository.GetUserByUsername(username);
            if (existingUser?.id != null && existingUser.id > 0)
            {
                return existingUser.id.Value;
            }
            
            // Create a new user if not found (with default role)
            var defaultRole = "Employee"; // Default role for Auth0 users
            _userRepository.CreateUser(username, "", defaultRole); // Empty password for Auth0 users
            
            // Get the newly created user
            var newUser = _userRepository.GetUserByUsername(username);
            if (newUser?.id != null && newUser.id > 0)
            {
                return newUser.id.Value;
            }
            
            // Fallback: return 1 if something goes wrong (you may want to handle this differently)
            return 1;
        }

        // Order endpoint - Refactored and robust
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostSaveOrder()
        {
            try
            {
                // Read request body
                Request.EnableBuffering();
                Request.Body.Position = 0;
                
                string body;
                using (var reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8, leaveOpen: true))
                {
                    body = await reader.ReadToEndAsync();
                }
                Request.Body.Position = 0;

                if (string.IsNullOrWhiteSpace(body))
                {
                    return new JsonResult(new { success = false, message = "Order data is required." }) { StatusCode = 400 };
                }

                // Deserialize JSON
                CreateOrderDTO? createOrderDto;
                try
                {
                    createOrderDto = JsonSerializer.Deserialize<CreateOrderDTO>(body, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                }
                catch (JsonException ex)
                {
                    return new JsonResult(new { success = false, message = $"Invalid JSON format: {ex.Message}" }) { StatusCode = 400 };
                }

                if (createOrderDto == null)
                {
                    return new JsonResult(new { success = false, message = "Invalid order data." }) { StatusCode = 400 };
                }

                // Check if user is authenticated
                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    return new JsonResult(new { success = false, message = "User not authenticated." }) { StatusCode = 401 };
                }

                // Get or create user ID from Auth0 claims
                var userId = GetOrCreateUserIdFromAuth0();
                if (userId <= 0)
                {
                    return new JsonResult(new { success = false, message = "Unable to determine user. Please log in again." }) { StatusCode = 401 };
                }

                // Set user ID
                createOrderDto.UserId = userId;

                // Validate and save order
                var result = _orderService.CreateOrder(createOrderDto);

                if (!result.IsValid)
                {
                    return new JsonResult(new { success = false, message = result.ErrorMessage ?? "Error creating order." }) { StatusCode = 400 };
                }

                return new JsonResult(new { success = true, message = "Order saved successfully!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Error: {ex.Message}" }) { StatusCode = 500 };
            }
        }
    }
}
