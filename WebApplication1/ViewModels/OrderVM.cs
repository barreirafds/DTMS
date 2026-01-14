namespace DTMS.ViewModels
{
    public class OrderVM
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderItemVM> Items { get; set; } = new();
        public decimal Total { get; set; }
    }

    public class OrderItemVM
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
    }
}

