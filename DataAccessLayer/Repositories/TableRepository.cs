using System;
using System.Collections.Generic;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Models;
using Microsoft.Data.SqlClient;

namespace DataAcessLayer.Repositories;

public class TableRepository : ITableRepository
{
    public string connString = "Server=mssqlstud.fhict.local;Database=dbi570286_dbdtms1;User Id=dbi570286_dbdtms1;Password=root1234;TrustServerCertificate=True;";

    public List<table> tables2 = new List<table>();
    public List<table> GetTables()
    {
        var tables = new List<table>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "SELECT [id], [number], [seats], [status] FROM [table] ORDER BY [id];";
            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tables.Add(new table
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    number = reader.GetInt32(reader.GetOrdinal("number")),
                    seats = reader.GetInt32(reader.GetOrdinal("seats")),
                    status = reader.GetString(reader.GetOrdinal("status"))
                });
            }
        }
        catch (Exception)
        {
            // Return empty list if database connection fails
            return tables;
        }
        return tables;
    }

    public table? GetTable(int id)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "SELECT [id], [number], [seats], [status] FROM [table] WHERE [id]=@id;";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new table
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    number = reader.GetInt32(reader.GetOrdinal("number")),
                    seats = reader.GetInt32(reader.GetOrdinal("seats")),
                    status = reader.GetString(reader.GetOrdinal("status"))
                };
            }
        }
        catch (Exception)
        {
            // Return null if database connection fails
        }
        return null;
    }

    public void CreateTable(int number, int seats, string status)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "INSERT INTO [table] ([number], [seats], [status]) VALUES (@number, @seats, @status);";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@number", number);
            cmd.Parameters.AddWithValue("@seats", seats);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.ExecuteNonQuery();
        }
        catch (Exception)
        {
            // Ignore error if database connection fails
        }
    }

    public void UpdateTable(table t)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "UPDATE [table] SET [number]=@number, [seats]=@seats, [status]=@status WHERE [id]=@id;";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@number", t.number);
            cmd.Parameters.AddWithValue("@seats", t.seats);
            cmd.Parameters.AddWithValue("@status", t.status);
            cmd.Parameters.AddWithValue("@id", t.id);
            cmd.ExecuteNonQuery();
        }
        catch (Exception)
        {
            // Ignore error if database connection fails
        }
    }

    public void DeleteTable(int id)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "DELETE FROM [table] WHERE [id]=@id;";
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

