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
        // Validação: username e password são obrigatórios
        if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
        {
            return ValidationResult.Failure("Invalid Credentials");
        }

        // Validação hardcoded temporária (para desenvolvimento)
        if (loginDto.Username == "user" && loginDto.Password == "password")
        {
            return ValidationResult.Success();
        }

        // Verificar no banco de dados
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
        // Validação: todos os campos são obrigatórios
        if (string.IsNullOrWhiteSpace(registerDto.Username) ||
            string.IsNullOrWhiteSpace(registerDto.Password) ||
            string.IsNullOrWhiteSpace(registerDto.ConfirmPassword) ||
            string.IsNullOrWhiteSpace(registerDto.Role))
        {
            return ValidationResult.Failure("All fields are required.");
        }

        // Validação: passwords devem coincidir
        if (registerDto.Password != registerDto.ConfirmPassword)
        {
            return ValidationResult.Failure("Passwords do not match.", nameof(registerDto.ConfirmPassword));
        }

        // Verificar se o utilizador já existe
        var users = _userRepository.GetUsers();
        if (users.Any(u => u.user1 == registerDto.Username))
        {
            return ValidationResult.Failure("Username already exists.", nameof(registerDto.Username));
        }

        _userRepository.CreateUser(registerDto.Username, registerDto.Password, registerDto.Role);
        return ValidationResult.Success();
    }
}

