using BusinessLogicLayer.DTOs;

namespace DTMS.Mappers
{
    public class OrderVMMappers
    {
        public static ViewModels.OrderVM ToViewModel(OrderDTO orderDTO)
        {
            return new ViewModels.OrderVM
            {
                Id = orderDTO.Id,
                TableId = orderDTO.TableId,
                UserId = orderDTO.UserId,
                Status = orderDTO.Status,
                CreatedAt = orderDTO.CreatedAt,
                Items = orderDTO.Items.Select(item => OrderItemVMMappers.ToViewModel(item)).ToList(),
                Total = orderDTO.Total
            };
        }

        public static List<ViewModels.OrderVM> ToViewModelList(List<OrderDTO> orderDTOs)
        {
            return orderDTOs.Select(ToViewModel).ToList();
        }
    }

    public class OrderItemVMMappers
    {
        public static ViewModels.OrderItemVM ToViewModel(OrderItemDTO orderItemDTO)
        {
            return new ViewModels.OrderItemVM
            {
                Id = orderItemDTO.Id,
                OrderId = orderItemDTO.OrderId,
                ProductId = orderItemDTO.ProductId,
                ProductName = orderItemDTO.ProductName,
                Quantity = orderItemDTO.Quantity,
                Price = orderItemDTO.Price,
                Subtotal = orderItemDTO.Subtotal
            };
        }

        public static List<ViewModels.OrderItemVM> ToViewModelList(List<OrderItemDTO> orderItemDTOs)
        {
            return orderItemDTOs.Select(ToViewModel).ToList();
        }
    }
}

