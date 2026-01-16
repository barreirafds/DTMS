using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Services;
using FluentAssertions;
using Moq;
using Xunit;
using MockData = UnitTests.MockData.MockData;

namespace UnitTests.Services;

public class TableServiceTests
{
    private readonly Mock<ITableRepository> _mockTableRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly TableService _tableService;

    public TableServiceTests()
    {
        _mockTableRepository = new Mock<ITableRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _tableService = new TableService(_mockTableRepository.Object, _mockOrderRepository.Object);
    }

    [Fact]
    public void GetAllTables_ShouldReturnMockTables()
    {
        // Arrange
        var mockTables = UnitTests.MockData.MockData.GetMockTables();
        
        _mockTableRepository
            .Setup(repo => repo.GetTables())
            .Returns(mockTables);

        // Act
        var result = _tableService.GetAllTables();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Number.Should().Be(1);
        result[0].Seats.Should().Be(4);
        result[1].Number.Should().Be(2);
        result[1].Seats.Should().Be(6);
    }
}

