using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Abstractions;

public interface IOrderRepository
{
    int CreateOrder(order order);
    void CreateOrderItem(order_item orderItem);
    List<order> GetOrders();
    order? GetOrder(int id);
    List<order_item> GetOrderItems(int orderId);
}

