using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Abstractions;

public interface IUserRepository
{
    List<user> GetUsers();
    user? GetUser(int id);
    user? GetUserByUsername(string username);
    void CreateUser(string username, string password, string role);
    void UpdateUser(user user);
    void DeleteUser(int id);
}

