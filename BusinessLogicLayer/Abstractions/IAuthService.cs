using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Abstractions;

public interface IAuthService
{
    ValidationResult ValidateCredentials(LoginDTO loginDto);
    ValidationResult RegisterUser(RegisterDTO registerDto);
}

