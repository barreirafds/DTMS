using BusinessLogicLayer.Abstractions;
using DataAcessLayer;
using DataAcessLayer.Models;

namespace BusinessLogicLayer.Services;

public class UserService : IUserService
{
    private readonly userconn _userConn;

    public UserService()
    {
        _userConn = new userconn();
    }

    public List<user> GetAllUsers()
    {
        return _userConn.GetUsers();
    }

    public user? GetUserById(int id)
    {
        return _userConn.GetUser(id);
    }

    public void CreateUser(string username, string password, string role)
    {
        _userConn.CreateUser(username, password, role);
    }

    public void UpdateUser(user user)
    {
        _userConn.UpdateUser(user);
    }

    public void DeleteUser(int id)
    {
        _userConn.DeleteUser(id);
    }
}

