using System;
using System.Collections.Generic;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Models;
using MySql.Data.MySqlClient;

namespace DataAcessLayer.Repositories;

public class UserRepository : IUserRepository
{
    public string connString = "server=localhost;port=3306;database=dtms;user=root;password=root;";

    public List<user> GetUsers()
    {
        var users = new List<user>();
        using var conn = new MySqlConnection(connString);
        conn.Open();

        const string query = "SELECT `id`, `user`, `password`, `role` FROM `user` ORDER BY `id`;";
        using var cmd = new MySqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            users.Add(new user
            {
                id = reader.IsDBNull(0) ? null : reader.GetInt32(0),
                user1 = reader.IsDBNull(1) ? null : reader.GetString(1),
                password = reader.IsDBNull(2) ? null : reader.GetString(2),
                role = reader.IsDBNull(3) ? null : reader.GetString(3)
            });
        }
        return users;
    }

    public user? GetUser(int id)
    {
        using var conn = new MySqlConnection(connString);
        conn.Open();

        const string query = "SELECT `id`, `user`, `password`, `role` FROM `user` WHERE `id`=@id;";
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new user
            {
                id = reader.IsDBNull(0) ? null : reader.GetInt32(0),
                user1 = reader.IsDBNull(1) ? null : reader.GetString(1),
                password = reader.IsDBNull(2) ? null : reader.GetString(2),
                role = reader.IsDBNull(3) ? null : reader.GetString(3)
            };
        }
        return null;
    }

    public void CreateUser(string username, string password, string role)
    {
        using var conn = new MySqlConnection(connString);
        conn.Open();

        const string query = "INSERT INTO `user` (`user`, `password`, `role`) VALUES (@username, @password, @role);";
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", password);
        cmd.Parameters.AddWithValue("@role", role);
        cmd.ExecuteNonQuery();
    }

    public void UpdateUser(user u)
    {
        if (u.id == null || u.id == 0)
        {
            return;
        }

        using var conn = new MySqlConnection(connString);
        conn.Open();

        const string query = "UPDATE `user` SET `user`=@username, `password`=@password, `role`=@role WHERE `id`=@id;";
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@username", u.user1 ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@password", u.password ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@role", u.role ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@id", u.id.Value);
        cmd.ExecuteNonQuery();
    }

    public void DeleteUser(int id)
    {
        using var conn = new MySqlConnection(connString);
        conn.Open();

        const string query = "DELETE FROM `user` WHERE `id`=@id;";
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
}

