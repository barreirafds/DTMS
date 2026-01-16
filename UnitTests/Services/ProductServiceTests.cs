using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.Services;
using FluentAssertions;
using Moq;
using Xunit;
using MockData = UnitTests.MockData.MockData;

namespace UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _productService = new ProductService(_mockProductRepository.Object);
    }

    [Fact]
    public void GetAllProducts_ShouldReturnMockProducts()
    {
        // Arrange
        var mockProducts = UnitTests.MockData.MockData.GetMockProducts();
        
        _mockProductRepository
            .Setup(repo => repo.GetProducts())
            .Returns(mockProducts);

        // Act
        var result = _productService.GetAllProducts();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);
        result[0].name.Should().Be("Pizza Margherita");
        result[0].price.Should().Be(12.50m);
        result[1].name.Should().Be("Coca-Cola");
        result[2].name.Should().Be("Classic Burger");
        result[3].name.Should().Be("Water");
    }

    [Fact]
    public void GetProductById_ShouldReturnProduct_WhenExists()
    {
        // Arrange
        var mockProducts = UnitTests.MockData.MockData.GetMockProducts();
        var productId = 1;
        
        _mockProductRepository
            .Setup(repo => repo.GetProduct(productId))
            .Returns(mockProducts.First(p => p.id == productId));

        // Act
        var result = _productService.GetProductById(productId);

        // Assert
        result.Should().NotBeNull();
        result!.id.Should().Be(1);
        result.name.Should().Be("Pizza Margherita");
        result.price.Should().Be(12.50m);
    }
}

