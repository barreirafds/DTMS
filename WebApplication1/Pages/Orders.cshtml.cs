using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DTMS.Pages
{
    public class OrdersModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly ITableService _tableService;
        private readonly IProductService _productService;

        public OrdersModel(IOrderService orderService, ITableService tableService, IProductService productService)
        {
            _orderService = orderService;
            _tableService = tableService;
            _productService = productService;
        }

        [BindProperty(SupportsGet = true)]
        public int? TableId { get; set; }

        [BindProperty]
        public int SelectedTableId { get; set; }

        [BindProperty]
        public int SelectedOrderId { get; set; }

        [BindProperty]
        public int SelectedProductId { get; set; }

        [BindProperty]
        public int Quantity { get; set; } = 1;

        public List<TableDTO> Tables { get; private set; } = new();
        public List<OrderDTO> Orders { get; private set; } = new();
        public List<ProductDTO> Products { get; private set; } = new();

        public void OnGet()
        {
            LoadData();
        }

        public IActionResult OnPostOpenOrder()
        {
            TableId = TableId > 0 ? TableId : null;
            var targetTableId = SelectedTableId > 0 ? SelectedTableId : TableId ?? 0;

            if (targetTableId <= 0)
            {
                ModelState.AddModelError(nameof(SelectedTableId), "Select a valid table to open an order.");
                LoadData();
                return Page();
            }

            var result = _orderService.CreateOrder(new CreateOrderDTO
            {
                TableId = targetTableId
            });

            if (!result.Validation.IsValid)
            {
                ModelState.AddModelError(result.Validation.FieldName ?? string.Empty, result.Validation.ErrorMessage ?? string.Empty);
                LoadData();
                return Page();
            }

            return RedirectToPage(new { tableId = targetTableId });
        }

        public IActionResult OnPostAddItem()
        {
            TableId = TableId > 0 ? TableId : null;
            var validation = _orderService.AddItemToOrder(new AddOrderItemDTO
            {
                OrderId = SelectedOrderId,
                ProductId = SelectedProductId,
                Quantity = Quantity
            });

            if (!validation.IsValid)
            {
                ModelState.AddModelError(validation.FieldName ?? string.Empty, validation.ErrorMessage ?? string.Empty);
                LoadData();
                return Page();
            }

            return RedirectToPage(new { tableId = TableId ?? 0 });
        }

        public IActionResult OnPostRemoveItem(int orderItemId)
        {
            _orderService.RemoveItemFromOrder(orderItemId);
            return RedirectToPage(new { tableId = TableId ?? 0 });
        }

        private void LoadData()
        {
            Tables = _tableService.GetAllTables();
            Products = _productService.GetAllProducts();

            if (TableId.HasValue && TableId.Value > 0)
            {
                Orders = _orderService.GetOrdersForTable(TableId.Value);
                SelectedTableId = TableId.Value;
            }
        }
    }
}
