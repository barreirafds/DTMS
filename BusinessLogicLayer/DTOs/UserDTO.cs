namespace BusinessLogicLayer.DTOs;

public class UserDTO
{
    public int? Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class CreateUserDTO
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class LoginDTO
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterDTO
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

