using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Abstractions;

public interface IOrderService
{
    List<OrderDTO> GetOrdersForTable(int tableId);
    (ValidationResult Validation, OrderDTO? Order) CreateOrder(CreateOrderDTO createOrderDto);
    ValidationResult AddItemToOrder(AddOrderItemDTO addOrderItemDto);
    ValidationResult RemoveItemFromOrder(int orderItemId);
}
