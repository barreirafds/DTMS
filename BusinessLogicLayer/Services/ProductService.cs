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
        try
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
        catch (Exception)
        {
            return new List<ProductDTO>();
        }
    }

    public ProductDTO? GetProductById(int id)
    {
        try
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
        catch (Exception)
        {
            return null;
        }
    }

    public ValidationResult CreateProduct(CreateProductDTO createProductDto)
    {
        if (string.IsNullOrWhiteSpace(createProductDto.name) || string.IsNullOrWhiteSpace(createProductDto.category))
        {
            return ValidationResult.Failure("Product name and category are required.");
        }

        if (createProductDto.price <= 0)
        {
            return ValidationResult.Failure("Product price must be greater than 0.", nameof(createProductDto.price));
        }

        try
        {
            _productRepository.CreateProduct(
                createProductDto.name,
                createProductDto.description,
                createProductDto.price,
                createProductDto.category
            );

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            return ValidationResult.Failure($"Error creating product: {ex.Message}");
        }
    }

    public void UpdateProduct(ProductDTO productDto)
    {
        try
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
        catch (Exception)
        {
            // Silently fail to prevent application crash
        }
    }

    public void DeleteProduct(int id)
    {
        try
        {
            _productRepository.DeleteProduct(id);
        }
        catch (Exception)
        {
            // Silently fail to prevent application crash
        }
    }
}

