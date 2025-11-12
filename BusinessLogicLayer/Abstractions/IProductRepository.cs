using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Abstractions;

public interface IProductRepository
{
    List<product> GetProducts();
    product? GetProduct(int id);
    void CreateProduct(string name, string? description, decimal price, string category);
    void UpdateProduct(product product);
    void DeleteProduct(int id);
}

