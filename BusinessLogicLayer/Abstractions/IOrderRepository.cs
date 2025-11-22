using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Abstractions;

public interface IOrderRepository
{
    List<order> GetOrdersByTable(int tableId);
    order? GetOrderById(int orderId);
    int CreateOrder(int tableId);
    void AddOrderItem(int orderId, int productId, int quantity, decimal unitPrice);
    void RemoveOrderItem(int orderItemId);
}
