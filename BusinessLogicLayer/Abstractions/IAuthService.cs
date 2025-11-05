using DataAcessLayer.Models;

namespace BusinessLogicLayer.Abstractions;

public interface IAuthService
{
    bool ValidateCredentials(string username, string password);
    bool RegisterUser(string username, string password, string confirmPassword, string role, out string? errorMessage);
}

