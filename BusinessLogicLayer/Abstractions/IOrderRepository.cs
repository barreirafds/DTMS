using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Abstractions;

public interface IOrderRepository
{
    int CreateOrderWithItems(order order, List<order_item> orderItems);
    int CreateOrder(order order);
    void CreateOrderItem(order_item orderItem);
    List<order> GetOrders();
    order? GetOrder(int id);
    List<order> GetOrdersByTableId(int tableId);
    order? GetPendingOrderByTableId(int tableId);
    List<order_item> GetOrderItems(int orderId);
    void UpdateOrderStatus(int orderId, string status);
}

