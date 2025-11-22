using System;
using System.Collections.Generic;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Models;
using Microsoft.Data.SqlClient;

namespace DataAcessLayer.Repositories;

public class OrderRepository : IOrderRepository
{
    public string connString = "Server=mssqlstud.fhict.local;Database=dbi570286_dbdtms1;User Id=dbi570286_dbdtms1;Password=root1234;TrustServerCertificate=True;";

    public List<order> GetOrdersByTable(int tableId)
    {
        var orders = new Dictionary<int, order>();

        using var conn = new SqlConnection(connString);
        conn.Open();

        const string query = @"SELECT o.id AS order_id, o.table_id, o.created_at,
                                       oi.id AS order_item_id, oi.product_id, oi.quantity, oi.unit_price,
                                       p.name AS product_name
                                FROM [order] o
                                LEFT JOIN [order_item] oi ON oi.order_id = o.id
                                LEFT JOIN [product] p ON p.id = oi.product_id
                                WHERE o.table_id = @tableId
                                ORDER BY o.created_at, oi.id;";

        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@tableId", tableId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var orderId = reader.GetInt32(reader.GetOrdinal("order_id"));
            if (!orders.TryGetValue(orderId, out var ord))
            {
                ord = new order
                {
                    id = orderId,
                    table_id = reader.GetInt32(reader.GetOrdinal("table_id")),
                    created_at = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    items = new List<order_item>()
                };
                orders.Add(orderId, ord);
            }

            if (!reader.IsDBNull(reader.GetOrdinal("order_item_id")))
            {
                ord.items.Add(new order_item
                {
                    id = reader.GetInt32(reader.GetOrdinal("order_item_id")),
                    order_id = orderId,
                    product_id = reader.GetInt32(reader.GetOrdinal("product_id")),
                    quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                    unit_price = reader.GetDecimal(reader.GetOrdinal("unit_price")),
                    product = new product
                    {
                        id = reader.GetInt32(reader.GetOrdinal("product_id")),
                        name = reader.IsDBNull(reader.GetOrdinal("product_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("product_name"))
                    }
                });
            }
        }

        return new List<order>(orders.Values);
    }

    public order? GetOrderById(int orderId)
    {
        using var conn = new SqlConnection(connString);
        conn.Open();

        const string query = @"SELECT o.id AS order_id, o.table_id, o.created_at,
                                       oi.id AS order_item_id, oi.product_id, oi.quantity, oi.unit_price,
                                       p.name AS product_name
                                FROM [order] o
                                LEFT JOIN [order_item] oi ON oi.order_id = o.id
                                LEFT JOIN [product] p ON p.id = oi.product_id
                                WHERE o.id = @orderId
                                ORDER BY oi.id;";

        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId", orderId);

        using var reader = cmd.ExecuteReader();
        order? ord = null;

        while (reader.Read())
        {
            if (ord == null)
            {
                ord = new order
                {
                    id = reader.GetInt32(reader.GetOrdinal("order_id")),
                    table_id = reader.GetInt32(reader.GetOrdinal("table_id")),
                    created_at = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    items = new List<order_item>()
                };
            }

            if (!reader.IsDBNull(reader.GetOrdinal("order_item_id")))
            {
                ord.items.Add(new order_item
                {
                    id = reader.GetInt32(reader.GetOrdinal("order_item_id")),
                    order_id = orderId,
                    product_id = reader.GetInt32(reader.GetOrdinal("product_id")),
                    quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                    unit_price = reader.GetDecimal(reader.GetOrdinal("unit_price")),
                    product = new product
                    {
                        id = reader.GetInt32(reader.GetOrdinal("product_id")),
                        name = reader.IsDBNull(reader.GetOrdinal("product_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("product_name"))
                    }
                });
            }
        }

        return ord;
    }

    public int CreateOrder(int tableId)
    {
        using var conn = new SqlConnection(connString);
        conn.Open();

        const string query = "INSERT INTO [order] ([table_id], [created_at]) OUTPUT INSERTED.[id] VALUES (@tableId, SYSDATETIME());";
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@tableId", tableId);

        return (int)cmd.ExecuteScalar();
    }

    public void AddOrderItem(int orderId, int productId, int quantity, decimal unitPrice)
    {
        using var conn = new SqlConnection(connString);
        conn.Open();

        const string query = "INSERT INTO [order_item] ([order_id], [product_id], [quantity], [unit_price]) VALUES (@orderId, @productId, @quantity, @unitPrice);";
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId", orderId);
        cmd.Parameters.AddWithValue("@productId", productId);
        cmd.Parameters.AddWithValue("@quantity", quantity);
        cmd.Parameters.AddWithValue("@unitPrice", unitPrice);
        cmd.ExecuteNonQuery();
    }

    public void RemoveOrderItem(int orderItemId)
    {
        using var conn = new SqlConnection(connString);
        conn.Open();

        const string query = "DELETE FROM [order_item] WHERE [id]=@orderItemId;";
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderItemId", orderItemId);
        cmd.ExecuteNonQuery();
    }
}
