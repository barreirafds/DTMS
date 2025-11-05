using BusinessLogicLayer.Abstractions;
using DataAcessLayer;
using DataAcessLayer.Models;

namespace BusinessLogicLayer.Services;

public class ProductService : IProductService
{
    private readonly productconn _productConn;

    public ProductService()
    {
        _productConn = new productconn();
    }

    public List<product> GetAllProducts()
    {
        return _productConn.GetProducts();
    }

    public product? GetProductById(int id)
    {
        return _productConn.GetProduct(id);
    }

    public void CreateProduct(string name, string? description, decimal price, string category)
    {
        _productConn.CreateProduct(name, description, price, category);
    }

    public void UpdateProduct(product product)
    {
        _productConn.UpdateProduct(product);
    }

    public void DeleteProduct(int id)
    {
        _productConn.DeleteProduct(id);
    }
}

