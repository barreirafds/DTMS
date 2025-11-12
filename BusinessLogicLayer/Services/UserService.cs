using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public List<UserDTO> GetAllUsers()
    {
        var users = _userRepository.GetUsers();
        return users.Select(u => new UserDTO
        {
            Id = u.id,
            Username = u.user1 ?? string.Empty,
            Password = u.password,
            Role = u.role ?? string.Empty
        }).ToList();
    }

    public UserDTO? GetUserById(int id)
    {
        var user = _userRepository.GetUser(id);
        if (user == null) return null;

        return new UserDTO
        {
            Id = user.id,
            Username = user.user1 ?? string.Empty,
            Password = user.password,
            Role = user.role ?? string.Empty
        };
    }

    public ValidationResult CreateUser(CreateUserDTO createUserDto)
    {
        // Validação: todos os campos são obrigatórios
        if (string.IsNullOrWhiteSpace(createUserDto.Username) ||
            string.IsNullOrWhiteSpace(createUserDto.Password) ||
            string.IsNullOrWhiteSpace(createUserDto.Role))
        {
            return ValidationResult.Failure("All fields are required.");
        }

        // Verificar se o utilizador já existe
        var users = _userRepository.GetUsers();
        if (users.Any(u => u.user1 == createUserDto.Username))
        {
            return ValidationResult.Failure("Username already exists.", nameof(createUserDto.Username));
        }

        _userRepository.CreateUser(createUserDto.Username, createUserDto.Password, createUserDto.Role);
        return ValidationResult.Success();
    }

    public void UpdateUser(UserDTO userDto)
    {
        if (userDto.Id == null || userDto.Id == 0)
        {
            return;
        }

        var user = new user
        {
            id = userDto.Id,
            user1 = userDto.Username,
            password = userDto.Password,
            role = userDto.Role
        };

        _userRepository.UpdateUser(user);
    }

    public void DeleteUser(int id)
    {
        if (id <= 0)
        {
            return;
        }

        _userRepository.DeleteUser(id);
    }
}

