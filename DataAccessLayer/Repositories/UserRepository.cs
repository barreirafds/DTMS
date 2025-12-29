using System;
using System.Collections.Generic;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Models;
using Microsoft.Data.SqlClient;

namespace DataAcessLayer.Repositories;

public class UserRepository : IUserRepository
{
    public string connString = "Server=mssqlstud.fhict.local;Database=dbi570286_dbdtms1;User Id=dbi570286_dbdtms1;Password=root1234;TrustServerCertificate=True;";

    public List<user> GetUsers()
    {
        var users = new List<user>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "SELECT [id], [user], [password], [role] FROM [user] ORDER BY [id];";
            using var cmd = new SqlCommand(query, conn);
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
        }
        catch (Exception)
        {
            // Return empty list if database connection fails
            return users;
        }
        return users;
    }

    public user? GetUser(int id)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "SELECT [id], [user], [password], [role] FROM [user] WHERE [id]=@id;";
            using var cmd = new SqlCommand(query, conn);
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
        }
        catch (Exception)
        {
            // Return null if database connection fails
        }
        return null;
    }

    public void CreateUser(string username, string password, string role)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "INSERT INTO [user] ([user], [password], [role]) VALUES (@username, @password, @role);";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@role", role);
            cmd.ExecuteNonQuery();
        }
        catch (Exception)
        {
            // Ignore error if database connection fails
        }
    }

    public void UpdateUser(user u)
    {
        if (u.id == null || u.id == 0)
        {
            return;
        }

        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "UPDATE [user] SET [user]=@username, [password]=@password, [role]=@role WHERE [id]=@id;";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", u.user1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@password", u.password ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@role", u.role ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@id", u.id.Value);
            cmd.ExecuteNonQuery();
        }
        catch (Exception)
        {
            // Ignore error if database connection fails
        }
    }

    public void DeleteUser(int id)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "DELETE FROM [user] WHERE [id]=@id;";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        catch (Exception)
        {
            // Ignore error if database connection fails
        }
    }
}

