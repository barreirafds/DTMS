using System;
using System.Collections.Generic;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Models;
using Microsoft.Data.SqlClient;

namespace DataAcessLayer.Repositories;

public class ProductRepository : IProductRepository
{
    public string connString = "Server=mssqlstud.fhict.local;Database=dbi570286_dbdtms1;User Id=dbi570286_dbdtms1;Password=Root1234;TrustServerCertificate=True;";

    public List<product> GetProducts()
    {
        var products = new List<product>();
        using var conn = new SqlConnection(connString);
        conn.Open();

        const string query = "SELECT [id], [name], [description], [price], [category], [created_at] FROM [product] ORDER BY [id];";
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            products.Add(new product
            {
                id = reader.GetInt32(0),
                name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                description = reader.IsDBNull(2) ? null : reader.GetString(2),
                price = reader.GetDecimal(3),
                category = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                created_at = reader.IsDBNull(5) ? null : reader.GetDateTime(5)
            });
        }
        return products;
    }

    public product? GetProduct(int id)
    {
        using var conn = new SqlConnection(connString);
        conn.Open();

        const string query = "SELECT [id], [name], [description], [price], [category], [created_at] FROM [product] WHERE [id]=@id;";
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new product
            {
                id = reader.GetInt32(0),
                name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                description = reader.IsDBNull(2) ? null : reader.GetString(2),
                price = reader.GetDecimal(3),
                category = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                created_at = reader.IsDBNull(5) ? null : reader.GetDateTime(5)
            };
        }
        return null;
    }

    public void CreateProduct(string name, string? description, decimal price, string category)
    {
        using var conn = new SqlConnection(connString);
        conn.Open();

        const string query = "INSERT INTO [product] ([name], [description], [price], [category]) VALUES (@name, @description, @price, @category);";
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@description", description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@price", price);
        cmd.Parameters.AddWithValue("@category", category);
        cmd.ExecuteNonQuery();
    }

    public void UpdateProduct(product p)
    {
        using var conn = new SqlConnection(connString);
        conn.Open();

        const string query = "UPDATE [product] SET [name]=@name, [description]=@description, [price]=@price, [category]=@category WHERE [id]=@id;";
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@name", p.name);
        cmd.Parameters.AddWithValue("@description", p.description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@price", p.price);
        cmd.Parameters.AddWithValue("@category", p.category);
        cmd.Parameters.AddWithValue("@id", p.id);
        cmd.ExecuteNonQuery();
    }

    public void DeleteProduct(int id)
    {
        using var conn = new SqlConnection(connString);
        conn.Open();

        const string query = "DELETE FROM [product] WHERE [id]=@id;";
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
}

