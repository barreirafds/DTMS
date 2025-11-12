using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public List<ProductDTO> GetAllProducts()
    {
        var products = _productRepository.GetProducts();
        return products.Select(p => new ProductDTO
        {
            id = p.id,
            name = p.name,
            description = p.description,
            price = p.price,
            category = p.category
        }).ToList();
    }

    public ProductDTO? GetProductById(int id)
    {
        var product = _productRepository.GetProduct(id);
        if (product == null) return null;

        return new ProductDTO
        {
            id = product.id,
            name = product.name,
            description = product.description,
            price = product.price,
            category = product.category
        };
    }

    public ValidationResult CreateProduct(CreateProductDTO createProductDto)
    {
        // Validação: nome e categoria são obrigatórios
        if (string.IsNullOrWhiteSpace(createProductDto.name) || string.IsNullOrWhiteSpace(createProductDto.category))
        {
            return ValidationResult.Failure("Product name and category are required.");
        }

        // Validação: preço deve ser maior que 0
        if (createProductDto.price <= 0)
        {
            return ValidationResult.Failure("Product price must be greater than 0.", nameof(createProductDto.price));
        }

        _productRepository.CreateProduct(
            createProductDto.name,
            createProductDto.description,
            createProductDto.price,
            createProductDto.category
        );

        return ValidationResult.Success();
    }

    public void UpdateProduct(ProductDTO productDto)
    {
        var product = new product
        {
            id = productDto.id,
            name = productDto.name,
            description = productDto.description,
            price = productDto.price,
            category = productDto.category
        };

        _productRepository.UpdateProduct(product);
    }

    public void DeleteProduct(int id)
    {
        _productRepository.DeleteProduct(id);
    }
}

