using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Abstractions;

public interface IOrderService
{
    ValidationResult CreateOrder(CreateOrderDTO createOrderDto);
    List<OrderDTO> GetAllOrders();
    OrderDTO? GetOrderById(int id);
    List<OrderDTO> GetOrdersByTableId(int tableId);
    OrderDTO? GetPendingOrderByTableId(int tableId);
    ValidationResult UpdateOrderStatus(int orderId, string status);
}

