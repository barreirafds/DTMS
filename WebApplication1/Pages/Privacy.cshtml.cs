using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.DTOs;

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

        public PrivacyModel(ILogger<PrivacyModel> logger, ITableRepository tableRepository, IProductService productService)
        {
            _logger = logger;
            _tableRepository = tableRepository;
            _productService = productService;
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

    }
    }
}
