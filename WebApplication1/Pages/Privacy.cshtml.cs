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

        // Order endpoint - Refactored and robust
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostSaveOrder()
        {
            try
            {
                _logger.LogInformation("OnPostSaveOrder: Request received");

                // Read JSON from request body
                CreateOrderDTO? createOrderDto;
                try
                {
                    createOrderDto = await Request.ReadFromJsonAsync<CreateOrderDTO>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OnPostSaveOrder: Failed to read JSON from request");
                    return new JsonResult(new { success = false, message = $"Failed to parse request: {ex.Message}" }) { StatusCode = 400 };
                }

                if (createOrderDto == null)
                {
                    _logger.LogWarning("OnPostSaveOrder: createOrderDto is null");
                    return new JsonResult(new { success = false, message = "Invalid order data. Request body is empty or invalid." }) { StatusCode = 400 };
                }

                // Validate basic data
                if (createOrderDto.TableId <= 0)
                {
                    _logger.LogWarning("OnPostSaveOrder: Invalid TableId: {TableId}", createOrderDto.TableId);
                    return new JsonResult(new { success = false, message = "Invalid table ID." }) { StatusCode = 400 };
                }

                if (createOrderDto.Items == null || createOrderDto.Items.Count == 0)
                {
                    _logger.LogWarning("OnPostSaveOrder: No items in order");
                    return new JsonResult(new { success = false, message = "Order must contain at least one item." }) { StatusCode = 400 };
                }

                _logger.LogInformation("OnPostSaveOrder: Deserialized order - TableId: {TableId}, ItemsCount: {Count}",
                    createOrderDto.TableId, createOrderDto.Items?.Count ?? 0);

                // Get user ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId) || userId <= 0)
                {
                    _logger.LogWarning("OnPostSaveOrder: User not authenticated or invalid user ID");
                    return new JsonResult(new { success = false, message = "User not authenticated." }) { StatusCode = 401 };
                }

                // Set user ID
                createOrderDto.UserId = userId;
                _logger.LogInformation("OnPostSaveOrder: User ID set to: {UserId}", userId);

                // Validate and save order
                var result = _orderService.CreateOrder(createOrderDto);

                if (!result.IsValid)
                {
                    _logger.LogWarning("OnPostSaveOrder: Order validation failed - {Error}", result.ErrorMessage);
                    return new JsonResult(new { success = false, message = result.ErrorMessage ?? "Error creating order." }) { StatusCode = 400 };
                }

                _logger.LogInformation("OnPostSaveOrder: Order saved successfully");
                return new JsonResult(new { success = true, message = "Order saved successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnPostSaveOrder: Exception occurred");
                return new JsonResult(new { success = false, message = $"Error: {ex.Message}" }) { StatusCode = 500 };
            }
        }
    }
}
