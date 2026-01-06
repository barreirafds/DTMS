using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.DTOs;
using System.Security.Claims;

namespace DTMS.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly ITableRepository _tableRepository;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public List<table> ShowTableEmployee { get; private set; } = new();
        public List<ProductDTO> Products { get; private set; } = new();

        [BindProperty]
        public int OrderTableId { get; set; }

        [BindProperty]
        public List<OrderItemInput> OrderItems { get; set; } = new();

        public PrivacyModel(ILogger<PrivacyModel> logger, ITableRepository tableRepository, IProductService productService, IOrderService orderService)
        {
            _logger = logger;
            _tableRepository = tableRepository;
            _productService = productService;
            _orderService = orderService;
        }

        public void OnGet()
        {
            ShowTableEmployee = _tableRepository.GetTables();
            Products = _productService.GetAllProducts();
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

        // Order endpoint - Save order to database
        public IActionResult OnPostSaveOrder()
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId) || userId <= 0)
            {
                TempData["ErrorMessage"] = "User not authenticated. Please log in again.";
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

            TempData["SuccessMessage"] = $"Order saved successfully for Table #{OrderTableId}!";
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
