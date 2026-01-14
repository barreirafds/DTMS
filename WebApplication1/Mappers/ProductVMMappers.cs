using BusinessLogicLayer.DTOs;

namespace DTMS.Mappers
{
    public class ProductVMMappers
    {
        public static ViewModels.ProductVM ToViewModel(ProductDTO productDTO)
        {
            return new ViewModels.ProductVM
            {
                Id = productDTO.id,
                Name = productDTO.name,
                Description = productDTO.description,
                Price = productDTO.price,
                Category = productDTO.category
            };
        }

        public static List<ViewModels.ProductVM> ToViewModelList(List<ProductDTO> productDTOs)
        {
            return productDTOs.Select(ToViewModel).ToList();
        }
    }
}

