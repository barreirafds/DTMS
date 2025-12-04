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
            throw new ArgumentException(); // change from validation result to argumentationException
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

        // Create order and items in a single transaction
        var order = new order
        {
            table_id = createOrderDto.TableId,
            user_id = createOrderDto.UserId,
            status = "Pending",
            created_at = DateTime.Now
        };

        var orderItems = new List<order_item>();
        foreach (var item in createOrderDto.Items)
        {
            orderItems.Add(new order_item
            {
                product_id = item.ProductId,
                qty = item.Quantity,
                price = item.Price
            });
        }

        try
        {
            var orderId = _orderRepository.CreateOrderWithItems(order, orderItems);
            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            return ValidationResult.Failure($"Error saving order to database: {ex.Message}");
        }
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

    public List<OrderDTO> GetOrdersByTableId(int tableId)
    {
        var orders = _orderRepository.GetOrdersByTableId(tableId);
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

    public OrderDTO? GetPendingOrderByTableId(int tableId)
    {
        var order = _orderRepository.GetPendingOrderByTableId(tableId);
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

    public ValidationResult UpdateOrderStatus(int orderId, string status)
    {
        if (orderId <= 0)
        {
            return ValidationResult.Failure("Order ID must be greater than 0.", nameof(orderId));
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            return ValidationResult.Failure("Status is required.", nameof(status));
        }

        try
        {
            _orderRepository.UpdateOrderStatus(orderId, status);
            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            return ValidationResult.Failure($"Error updating order status: {ex.Message}");
        }
    }
}

