using DataAcessLayer.Models;

namespace BusinessLogicLayer.Abstractions;

public interface IProductService
{
    List<product> GetAllProducts();
    product? GetProductById(int id);
    void CreateProduct(string name, string? description, decimal price, string category);
    void UpdateProduct(product product);
    void DeleteProduct(int id);
}

