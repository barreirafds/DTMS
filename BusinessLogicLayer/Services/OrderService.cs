using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Models;
using System.Linq;

namespace BusinessLogicLayer.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(IOrderRepository orderRepository, ITableRepository tableRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _tableRepository = tableRepository;
        _productRepository = productRepository;
    }

    public List<OrderDTO> GetOrdersForTable(int tableId)
    {
        var orders = _orderRepository.GetOrdersByTable(tableId);
        return orders.Select(MapToDto).ToList();
    }

    public (ValidationResult Validation, OrderDTO? Order) CreateOrder(CreateOrderDTO createOrderDto)
    {
        var table = _tableRepository.GetTable(createOrderDto.TableId);
        if (table == null)
        {
            return (ValidationResult.Failure("Table not found.", nameof(createOrderDto.TableId)), null);
        }

        foreach (var item in createOrderDto.Items)
        {
            if (item.Quantity <= 0)
            {
                return (ValidationResult.Failure("Quantity must be greater than 0.", nameof(item.Quantity)), null);
            }

            var product = _productRepository.GetProduct(item.ProductId);
            if (product == null)
            {
                return (ValidationResult.Failure($"Product with id {item.ProductId} was not found.", nameof(item.ProductId)), null);
            }
        }

        var orderId = _orderRepository.CreateOrder(createOrderDto.TableId);

        foreach (var item in createOrderDto.Items)
        {
            var product = _productRepository.GetProduct(item.ProductId)!;
            _orderRepository.AddOrderItem(orderId, item.ProductId, item.Quantity, product.price);
        }

        var order = _orderRepository.GetOrderById(orderId);
        return (ValidationResult.Success(), order == null ? null : MapToDto(order));
    }

    public ValidationResult AddItemToOrder(AddOrderItemDTO addOrderItemDto)
    {
        if (addOrderItemDto.Quantity <= 0)
        {
            return ValidationResult.Failure("Quantity must be greater than 0.", nameof(addOrderItemDto.Quantity));
        }

        var order = _orderRepository.GetOrderById(addOrderItemDto.OrderId);
        if (order == null)
        {
            return ValidationResult.Failure("Order not found.", nameof(addOrderItemDto.OrderId));
        }

        var product = _productRepository.GetProduct(addOrderItemDto.ProductId);
        if (product == null)
        {
            return ValidationResult.Failure("Product not found.", nameof(addOrderItemDto.ProductId));
        }

        _orderRepository.AddOrderItem(addOrderItemDto.OrderId, addOrderItemDto.ProductId, addOrderItemDto.Quantity, product.price);
        return ValidationResult.Success();
    }

    public ValidationResult RemoveItemFromOrder(int orderItemId)
    {
        _orderRepository.RemoveOrderItem(orderItemId);
        return ValidationResult.Success();
    }

    private static OrderDTO MapToDto(order ord)
    {
        return new OrderDTO
        {
            Id = ord.id,
            TableId = ord.table_id,
            CreatedAt = ord.created_at,
            Items = ord.items.Select(oi => new OrderItemDTO
            {
                Id = oi.id,
                ProductId = oi.product_id,
                ProductName = oi.product?.name ?? string.Empty,
                UnitPrice = oi.unit_price,
                Quantity = oi.quantity
            }).ToList()
        };
    }
}
