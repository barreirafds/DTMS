using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Abstractions;

public interface IProductService
{
    List<ProductDTO> GetAllProducts();
    ProductDTO? GetProductById(int id);
    ValidationResult CreateProduct(CreateProductDTO createProductDto);
    void UpdateProduct(ProductDTO productDto);
    void DeleteProduct(int id);
}

