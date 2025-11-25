using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.DTOs;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

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
        public OrderInputModel OrderInput { get; set; } = new();

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

        // Save order using model binding
        public IActionResult OnPostSaveOrder()
        {
            try
            {
                _logger.LogInformation("OnPostSaveOrder: Request received - TableId: {TableId}, ItemsCount: {Count}",
                    OrderInput.TableId, OrderInput.Items?.Count ?? 0);

                // Validate model
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("OnPostSaveOrder: Model validation failed");
                    ShowTableEmployee = _tableRepository.GetTables();
                    Products = _productService.GetAllProducts();
                    return Page();
                }

                // Validate basic data
                if (OrderInput.TableId <= 0)
                {
                    ModelState.AddModelError("OrderInput.TableId", "Invalid table ID.");
                    ShowTableEmployee = _tableRepository.GetTables();
                    Products = _productService.GetAllProducts();
                    return Page();
                }

                if (OrderInput.Items == null || OrderInput.Items.Count == 0)
                {
                    ModelState.AddModelError("OrderInput.Items", "Order must contain at least one item.");
                    ShowTableEmployee = _tableRepository.GetTables();
                    Products = _productService.GetAllProducts();
                    return Page();
                }

                // Get user ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId) || userId <= 0)
                {
                    _logger.LogWarning("OnPostSaveOrder: User not authenticated");
                    ModelState.AddModelError("", "User not authenticated.");
                    ShowTableEmployee = _tableRepository.GetTables();
                    Products = _productService.GetAllProducts();
                    return Page();
                }

                // Convert to DTO
                var createOrderDto = new CreateOrderDTO
                {
                    TableId = OrderInput.TableId,
                    UserId = userId,
                    Items = OrderInput.Items.Select(item => new CreateOrderItemDTO
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    }).ToList()
                };

                // Save order
                var result = _orderService.CreateOrder(createOrderDto);

                if (!result.IsValid)
                {
                    _logger.LogWarning("OnPostSaveOrder: Order validation failed - {Error}", result.ErrorMessage);
                    ModelState.AddModelError("", result.ErrorMessage ?? "Error creating order.");
                    ShowTableEmployee = _tableRepository.GetTables();
                    Products = _productService.GetAllProducts();
                    return Page();
                }

                _logger.LogInformation("OnPostSaveOrder: Order saved successfully");
                TempData["SuccessMessage"] = "Order saved successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnPostSaveOrder: Exception occurred");
                ModelState.AddModelError("", $"Error: {ex.Message}");
                ShowTableEmployee = _tableRepository.GetTables();
                Products = _productService.GetAllProducts();
                return Page();
            }
        }
    }

    // Input model for order form
    public class OrderInputModel
    {
        public int TableId { get; set; }
        public List<OrderItemInputModel> Items { get; set; } = new();
    }

    public class OrderItemInputModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
    }
}
