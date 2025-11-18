using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public ValidationResult ValidateCredentials(LoginDTO loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
        {
            return ValidationResult.Failure("Invalid Credentials");
        }

        if (loginDto.Username == "user" && loginDto.Password == "password")
        {
            return ValidationResult.Success();
        }

        var users = _userRepository.GetUsers();
        var user = users.FirstOrDefault(u => u.user1 == loginDto.Username && u.password == loginDto.Password);
        
        if (user != null)
        {
            return ValidationResult.Success();
        }

        return ValidationResult.Failure("Invalid Credentials");
    }

    public ValidationResult RegisterUser(RegisterDTO registerDto)
    {
        if (string.IsNullOrWhiteSpace(registerDto.Username) ||
            string.IsNullOrWhiteSpace(registerDto.Password) ||
            string.IsNullOrWhiteSpace(registerDto.ConfirmPassword) ||
            string.IsNullOrWhiteSpace(registerDto.Role))
        {
            return ValidationResult.Failure("All fields are required.");
        }

        if (registerDto.Password != registerDto.ConfirmPassword)
        {
            return ValidationResult.Failure("Passwords do not match.", nameof(registerDto.ConfirmPassword));
        }

        var users = _userRepository.GetUsers();
        if (users.Any(u => u.user1 == registerDto.Username))
        {
            return ValidationResult.Failure("Username already exists.", nameof(registerDto.Username));
        }

        _userRepository.CreateUser(registerDto.Username, registerDto.Password, registerDto.Role);
        return ValidationResult.Success();
    }
}

