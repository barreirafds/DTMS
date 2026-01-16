using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.Services;
using FluentAssertions;
using Moq;
using Xunit;
using MockData = UnitTests.MockData.MockData;

namespace UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockUserRepository.Object);
    }

    [Fact]
    public void GetAllUsers_ShouldReturnListOfUserDTOs_WhenUsersExist()
    {
        // Arrange
        var mockUsers = UnitTests.MockData.MockData.GetMockUsers();

        _mockUserRepository
            .Setup(repo => repo.GetUsers())
            .Returns(mockUsers);

        // Act
        var result = _userService.GetAllUsers();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Id.Should().Be(1);
        result[0].Username.Should().Be("admin");
        result[0].Role.Should().Be("Admin");
        result[1].Id.Should().Be(2);
        result[1].Username.Should().Be("waiter");
        result[1].Role.Should().Be("Waiter");

        _mockUserRepository.Verify(repo => repo.GetUsers(), Times.Once);
    }

    [Fact]
    public void GetUserById_ShouldReturnUserDTO_WhenUserExists()
    {
        // Arrange
        var mockUser = new user
        {
            id = 1,
            user1 = "testuser",
            password = "password123",
            role = "Admin"
        };

        _mockUserRepository
            .Setup(repo => repo.GetUser(1))
            .Returns(mockUser);

        // Act
        var result = _userService.GetUserById(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Username.Should().Be("testuser");
        result.Password.Should().Be("password123");
        result.Role.Should().Be("Admin");

        _mockUserRepository.Verify(repo => repo.GetUser(1), Times.Once);
    }

    [Fact]
    public void GetUserById_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetUser(999))
            .Returns((user?)null);

        // Act
        var result = _userService.GetUserById(999);

        // Assert
        result.Should().BeNull();
        _mockUserRepository.Verify(repo => repo.GetUser(999), Times.Once);
    }

    [Fact]
    public void CreateUser_ShouldReturnSuccess_WhenValidUserData()
    {
        // Arrange
        var createUserDto = new CreateUserDTO
        {
            Username = "newuser",
            Password = "password123",
            Role = "User"
        };

        var existingUsers = new List<user>();

        _mockUserRepository
            .Setup(repo => repo.GetUsers())
            .Returns(existingUsers);

        _mockUserRepository
            .Setup(repo => repo.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        // Act
        var result = _userService.CreateUser(createUserDto);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.ErrorMessage.Should().BeNullOrEmpty();

        _mockUserRepository.Verify(repo => repo.GetUsers(), Times.Once);
        _mockUserRepository.Verify(
            repo => repo.CreateUser("newuser", "password123", "User"),
            Times.Once);
    }

    [Fact]
    public void CreateUser_ShouldReturnFailure_WhenUsernameAlreadyExists()
    {
        // Arrange
        var createUserDto = new CreateUserDTO
        {
            Username = "existinguser",
            Password = "password123",
            Role = "User"
        };

        var existingUsers = new List<user>
        {
            new user { id = 1, user1 = "existinguser", password = "oldpass", role = "Admin" }
        };

        _mockUserRepository
            .Setup(repo => repo.GetUsers())
            .Returns(existingUsers);

        // Act
        var result = _userService.CreateUser(createUserDto);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Username already exists");
        result.FieldName.Should().Be("Username");

        _mockUserRepository.Verify(repo => repo.GetUsers(), Times.Once);
        _mockUserRepository.Verify(
            repo => repo.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public void CreateUser_ShouldReturnFailure_WhenRequiredFieldsAreEmpty()
    {
        // Arrange
        var createUserDto = new CreateUserDTO
        {
            Username = "",
            Password = "",
            Role = ""
        };

        // Act
        var result = _userService.CreateUser(createUserDto);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("All fields are required");

        _mockUserRepository.Verify(repo => repo.GetUsers(), Times.Never);
        _mockUserRepository.Verify(
            repo => repo.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public void UpdateUser_ShouldCallRepository_WhenValidUserData()
    {
        // Arrange
        var userDto = new UserDTO
        {
            Id = 1,
            Username = "updateduser",
            Password = "newpassword",
            Role = "Admin"
        };

        _mockUserRepository
            .Setup(repo => repo.UpdateUser(It.IsAny<user>()));

        // Act
        _userService.UpdateUser(userDto);

        // Assert
        _mockUserRepository.Verify(
            repo => repo.UpdateUser(It.Is<user>(u =>
                u.id == 1 &&
                u.user1 == "updateduser" &&
                u.password == "newpassword" &&
                u.role == "Admin")),
            Times.Once);
    }

    [Fact]
    public void UpdateUser_ShouldNotCallRepository_WhenIdIsNull()
    {
        // Arrange
        var userDto = new UserDTO
        {
            Id = null,
            Username = "testuser",
            Password = "password",
            Role = "User"
        };

        // Act
        _userService.UpdateUser(userDto);

        // Assert
        _mockUserRepository.Verify(
            repo => repo.UpdateUser(It.IsAny<user>()),
            Times.Never);
    }

    [Fact]
    public void UpdateUser_ShouldNotCallRepository_WhenIdIsZero()
    {
        // Arrange
        var userDto = new UserDTO
        {
            Id = 0,
            Username = "testuser",
            Password = "password",
            Role = "User"
        };

        // Act
        _userService.UpdateUser(userDto);

        // Assert
        _mockUserRepository.Verify(
            repo => repo.UpdateUser(It.IsAny<user>()),
            Times.Never);
    }

    [Fact]
    public void DeleteUser_ShouldCallRepository_WhenValidId()
    {
        // Arrange
        var userId = 1;

        _mockUserRepository
            .Setup(repo => repo.DeleteUser(userId));

        // Act
        _userService.DeleteUser(userId);

        // Assert
        _mockUserRepository.Verify(repo => repo.DeleteUser(userId), Times.Once);
    }

    [Fact]
    public void DeleteUser_ShouldNotCallRepository_WhenIdIsZero()
    {
        // Arrange
        var userId = 0;

        // Act
        _userService.DeleteUser(userId);

        // Assert
        _mockUserRepository.Verify(
            repo => repo.DeleteUser(It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public void DeleteUser_ShouldNotCallRepository_WhenIdIsNegative()
    {
        // Arrange
        var userId = -1;

        // Act
        _userService.DeleteUser(userId);

        // Assert
        _mockUserRepository.Verify(
            repo => repo.DeleteUser(It.IsAny<int>()),
            Times.Never);
    }
}

