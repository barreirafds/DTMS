using DataAcessLayer.Models;

namespace BusinessLogicLayer.Abstractions;

public interface IUserService
{
    List<user> GetAllUsers();
    user? GetUserById(int id);
    void CreateUser(string username, string password, string role);
    void UpdateUser(user user);
    void DeleteUser(int id);
}

