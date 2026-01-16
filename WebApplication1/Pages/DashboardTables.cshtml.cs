using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.DTOs;
using System.Security.Claims;

namespace DTMS.Pages
{
    public class DashboardTablesModel : PageModel
    {
        private readonly ILogger<DashboardTablesModel> _logger;
        private readonly ITableRepository _tableRepository;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IUserRepository _userRepository;

        public List<table> ShowTableEmployee { get; private set; } = new();
        public List<ProductDTO> Products { get; private set; } = new();

        [BindProperty]
        public int OrderTableId { get; set; }

        [BindProperty]
        public List<OrderItemInput> OrderItems { get; set; } = new();

        public DashboardTablesModel(ILogger<DashboardTablesModel> logger, ITableRepository tableRepository, IProductService productService, IOrderService orderService, IUserRepository userRepository)
        {
            _logger = logger;
            _tableRepository = tableRepository;
            _productService = productService;
            _orderService = orderService;
            _userRepository = userRepository;
        }

        public void OnGet()
        {
            try
            {
                ShowTableEmployee = _tableRepository.GetTables();
            }
            catch (Exception)
            {
                ShowTableEmployee = new List<table>();
            }

            try
            {
                Products = _productService.GetAllProducts();
            }
            catch (Exception)
            {
                Products = new List<ProductDTO>();
            }
        }

        public IActionResult OnGetTableOrders(int tableId)
        {
            // Get only the pending order for this table (most recent)
            var pendingOrder = _orderService.GetPendingOrderByTableId(tableId);
            
            if (pendingOrder == null)
            {
                return new JsonResult(new { hasPendingOrder = false, items = new List<object>() });
            }

            var items = pendingOrder.Items.Select(item => new
            {
                productId = item.ProductId,
                productName = item.ProductName,
                quantity = item.Quantity,
                price = item.Price
            }).ToList();

            return new JsonResult(new 
            { 
                hasPendingOrder = true,
                orderId = pendingOrder.Id,
                items = items 
            });
        }

        public string showtables()
        {
            // This method seems unused, but kept for compatibility
            return "Connection string is managed by repository";
        }

        // Function to get  the badge (color bg) of the status
        public string GetStatusBadgeClass(string? status)
        {
            var s = (status ?? "Available").ToLowerInvariant();
            
            if (s == "available") return "bg-success";
            else if (s == "occupied") return "bg-danger";
            else if (s == "reserved") return "bg-warning";
            else return "bg-secondary";
        }

        // Helper method to get or create user from Auth0 claims
        private int GetOrCreateUserIdFromAuth0()
        {
            try
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
            catch (Exception)
            {
                // Fallback: return 1 if database connection fails
                return 1;
            }
        }

        // Order endpoint - Save order to database
        public IActionResult OnPostSaveOrder()
        {
            // Check if user is authenticated
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                TempData["ErrorMessage"] = "User not authenticated. Please log in again.";
                OnGet();
                return RedirectToPage();
            }

            // Get or create user ID from Auth0 claims
            var userId = GetOrCreateUserIdFromAuth0();
            if (userId <= 0)
            {
                TempData["ErrorMessage"] = "Unable to determine user. Please log in again.";
                OnGet();
                return RedirectToPage();
            }

            // Validate order data
            if (OrderTableId <= 0)
            {
                TempData["ErrorMessage"] = "Table ID is required.";
                OnGet();
                return RedirectToPage();
            }

            if (OrderItems == null || OrderItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Order must contain at least one item.";
                OnGet();
                return RedirectToPage();
            }

            // Create DTO from form data
            var createOrderDto = new CreateOrderDTO
            {
                TableId = OrderTableId,
                UserId = userId,
                Items = OrderItems.Select(item => new CreateOrderItemDTO
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            };

            // Validate and save order
            var result = _orderService.CreateOrder(createOrderDto);

            if (!result.IsValid)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Error creating order.";
                OnGet();
                return RedirectToPage();
            }

            // Get table number for success message
            var table = _tableRepository.GetTable(OrderTableId);
            var tableNumber = table?.number ?? OrderTableId;
            TempData["SuccessMessage"] = $"Order saved successfully for Table #{tableNumber}!";
            return RedirectToPage();
        }

        // Pay order endpoint - Update order status to Paid
        public IActionResult OnPostPayOrder(int orderId)
        {
            if (orderId <= 0)
            {
                ModelState.AddModelError("", "Invalid order ID.");
                OnGet();
                return Page();
            }

            var result = _orderService.UpdateOrderStatus(orderId, "Paid");

            if (!result.IsValid)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "Error updating order status.");
                OnGet();
                return Page();
            }

            return RedirectToPage();
        }
    }

    // Helper class for form binding
    public class OrderItemInput
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

