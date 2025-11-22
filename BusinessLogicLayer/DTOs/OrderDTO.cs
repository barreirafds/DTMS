using System.Linq;

namespace BusinessLogicLayer.DTOs;

public class OrderDTO
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDTO> Items { get; set; } = new();
    // Calculated property for total (sum of all items)
    public decimal Total => Items.Sum(item => item.Price * item.Quantity);
}

public class OrderItemDTO
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    // Calculated property for subtotal
    public decimal Subtotal => Price * Quantity;
}

public class CreateOrderDTO
{
    public int TableId { get; set; }
    public int UserId { get; set; }
    public List<CreateOrderItemDTO> Items { get; set; } = new();
}

public class CreateOrderItemDTO
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

