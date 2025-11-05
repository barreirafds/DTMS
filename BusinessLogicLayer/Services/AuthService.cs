using BusinessLogicLayer.Abstractions;
using DataAcessLayer;
using DataAcessLayer.Models;

namespace BusinessLogicLayer.Services;

public class AuthService : IAuthService
{
    private readonly userconn _userConn;

    public AuthService()
    {
        _userConn = new userconn();
    }

    public bool ValidateCredentials(string username, string password)
    {
        // Por enquanto, validação simples hardcoded
        // TODO: Implementar validação real com banco de dados
        if (username == "user" && password == "password")
        {
            return true;
        }

        // Verificar no banco de dados
        var users = _userConn.GetUsers();
        var user = users.FirstOrDefault(u => u.user1 == username && u.password == password);
        return user != null;
    }

    public bool RegisterUser(string username, string password, string confirmPassword, string role, out string? errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || 
            string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(role))
        {
            errorMessage = "All fields are required.";
            return false;
        }

        if (password != confirmPassword)
        {
            errorMessage = "Passwords do not match.";
            return false;
        }

        // Verificar se o usuário já existe
        var users = _userConn.GetUsers();
        if (users.Any(u => u.user1 == username))
        {
            errorMessage = "Username already exists.";
            return false;
        }

        _userConn.CreateUser(username, password, role);
        return true;
    }
}

