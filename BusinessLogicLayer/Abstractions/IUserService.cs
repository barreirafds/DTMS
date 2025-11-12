using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Abstractions;

public interface IUserService
{
    List<UserDTO> GetAllUsers();
    UserDTO? GetUserById(int id);
    ValidationResult CreateUser(CreateUserDTO createUserDto);
    void UpdateUser(UserDTO userDto);
    void DeleteUser(int id);
}

