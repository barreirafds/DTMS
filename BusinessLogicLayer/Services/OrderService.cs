using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public ValidationResult CreateOrder(CreateOrderDTO createOrderDto)
    {
        // Validate table ID
        if (createOrderDto.TableId <= 0)
        {
            return ValidationResult.Failure("Table ID must be greater than 0.", nameof(createOrderDto.TableId));
        }

        // Validate user ID
        if (createOrderDto.UserId <= 0)
        {
            return ValidationResult.Failure("User ID must be greater than 0.", nameof(createOrderDto.UserId));
        }

        // Validate items
        if (createOrderDto.Items == null || createOrderDto.Items.Count == 0)
        {
            return ValidationResult.Failure("Order must contain at least one item.", nameof(createOrderDto.Items));
        }

        // Validate each item
        foreach (var item in createOrderDto.Items)
        {
            if (item.ProductId <= 0)
            {
                return ValidationResult.Failure("Product ID must be greater than 0.", nameof(item.ProductId));
            }

            if (item.Quantity <= 0)
            {
                return ValidationResult.Failure("Quantity must be greater than 0.", nameof(item.Quantity));
            }

            if (item.Price < 0)
            {
                return ValidationResult.Failure("Price cannot be negative.", nameof(item.Price));
            }

            // Verify product exists
            var product = _productRepository.GetProduct(item.ProductId);
            if (product == null)
            {
                return ValidationResult.Failure($"Product with ID {item.ProductId} does not exist.", nameof(item.ProductId));
            }
        }

        // Create order
        var order = new order
        {
            table_id = createOrderDto.TableId,
            user_id = createOrderDto.UserId,
            status = "Pending",
            created_at = DateTime.Now
        };

        var orderId = _orderRepository.CreateOrder(order);

        // Create order items
        foreach (var item in createOrderDto.Items)
        {
            var orderItem = new order_item
            {
                order_id = orderId,
                product_id = item.ProductId,
                qty = item.Quantity,
                price = item.Price
            };

            _orderRepository.CreateOrderItem(orderItem);
        }

        return ValidationResult.Success();
    }

    public List<OrderDTO> GetAllOrders()
    {
        var orders = _orderRepository.GetOrders();
        var ordersDto = new List<OrderDTO>();

        foreach (var order in orders)
        {
            var items = _orderRepository.GetOrderItems(order.id);
            var itemsDto = new List<OrderItemDTO>();

            foreach (var item in items)
            {
                var product = _productRepository.GetProduct(item.product_id);
                itemsDto.Add(new OrderItemDTO
                {
                    Id = item.id,
                    OrderId = item.order_id,
                    ProductId = item.product_id,
                    ProductName = product?.name ?? "Unknown",
                    Quantity = item.qty,
                    Price = item.price
                });
            }

            ordersDto.Add(new OrderDTO
            {
                Id = order.id,
                TableId = order.table_id,
                UserId = order.user_id,
                Status = order.status,
                CreatedAt = order.created_at,
                Items = itemsDto
            });
        }

        return ordersDto;
    }

    public OrderDTO? GetOrderById(int id)
    {
        var order = _orderRepository.GetOrder(id);
        if (order == null) return null;

        var items = _orderRepository.GetOrderItems(order.id);
        var itemsDto = items.Select(item =>
        {
            var product = _productRepository.GetProduct(item.product_id);
            return new OrderItemDTO
            {
                Id = item.id,
                OrderId = item.order_id,
                ProductId = item.product_id,
                ProductName = product?.name ?? "Unknown",
                Quantity = item.qty,
                Price = item.price
            };
        }).ToList();

        return new OrderDTO
        {
            Id = order.id,
            TableId = order.table_id,
            UserId = order.user_id,
            Status = order.status,
            CreatedAt = order.created_at,
            Items = itemsDto
        };
    }
}

