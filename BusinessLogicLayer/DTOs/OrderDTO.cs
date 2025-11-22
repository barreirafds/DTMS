using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.DTOs;

public class OrderItemDTO
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public class OrderDTO
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDTO> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.LineTotal);
}

public class CreateOrderItemDTO
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderDTO
{
    public int TableId { get; set; }
    public List<CreateOrderItemDTO> Items { get; set; } = new();
}

public class AddOrderItemDTO
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
