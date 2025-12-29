using System;
using System.Collections.Generic;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Models;
using Microsoft.Data.SqlClient;

namespace DataAcessLayer.Repositories;

public class OrderRepository : IOrderRepository
{
    public string connString = "Server=mssqlstud.fhict.local;Database=dbi570286_dbdtms1;User Id=dbi570286_dbdtms1;Password=root1234;TrustServerCertificate=True;";

    public int CreateOrderWithItems(order order, List<order_item> orderItems)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            
            using var transaction = conn.BeginTransaction();
            try
            {
                // Create order
                const string orderQuery = @"INSERT INTO [order] ([table_id], [user_id], [status], [created_at]) 
                                           OUTPUT INSERTED.id
                                           VALUES (@table_id, @user_id, @status, @created_at);";
                
                using var orderCmd = new SqlCommand(orderQuery, conn, transaction);
                orderCmd.Parameters.AddWithValue("@table_id", order.table_id);
                orderCmd.Parameters.AddWithValue("@user_id", order.user_id);
                orderCmd.Parameters.AddWithValue("@status", order.status);
                orderCmd.Parameters.AddWithValue("@created_at", order.created_at);

                var orderId = (int)orderCmd.ExecuteScalar();

                // Create order items
                const string itemQuery = @"INSERT INTO [order_item] ([order_id], [product_id], [qty], [price]) 
                                          VALUES (@order_id, @product_id, @qty, @price);";
                
                foreach (var item in orderItems)
                {
                    using var itemCmd = new SqlCommand(itemQuery, conn, transaction);
                    itemCmd.Parameters.AddWithValue("@order_id", orderId);
                    itemCmd.Parameters.AddWithValue("@product_id", item.product_id);
                    itemCmd.Parameters.AddWithValue("@qty", item.qty);
                    itemCmd.Parameters.AddWithValue("@price", item.price);
                    itemCmd.ExecuteNonQuery();
                }

                transaction.Commit();
                return orderId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception)
        {
            // Return 0 if database connection fails
            return 0;
        }
    }

    public int CreateOrder(order order)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = @"INSERT INTO [order] ([table_id], [user_id], [status], [created_at]) 
                                   OUTPUT INSERTED.id
                                   VALUES (@table_id, @user_id, @status, @created_at);";
            
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@table_id", order.table_id);
            cmd.Parameters.AddWithValue("@user_id", order.user_id);
            cmd.Parameters.AddWithValue("@status", order.status);
            cmd.Parameters.AddWithValue("@created_at", order.created_at);

            var orderId = (int)cmd.ExecuteScalar();
            return orderId;
        }
        catch (Exception)
        {
            // Return 0 if database connection fails
            return 0;
        }
    }

    public void CreateOrderItem(order_item orderItem)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = @"INSERT INTO [order_item] ([order_id], [product_id], [qty], [price]) 
                                   VALUES (@order_id, @product_id, @qty, @price);";
            
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@order_id", orderItem.order_id);
            cmd.Parameters.AddWithValue("@product_id", orderItem.product_id);
            cmd.Parameters.AddWithValue("@qty", orderItem.qty);
            cmd.Parameters.AddWithValue("@price", orderItem.price);
            
            cmd.ExecuteNonQuery();
        }
        catch (Exception)
        {
            // Ignore error if database connection fails
        }
    }

    public List<order> GetOrders()
    {
        var orders = new List<order>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "SELECT [id], [table_id], [user_id], [status], [created_at] FROM [order] ORDER BY [created_at] DESC;";
            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                orders.Add(new order
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    table_id = reader.GetInt32(reader.GetOrdinal("table_id")),
                    user_id = reader.GetInt32(reader.GetOrdinal("user_id")),
                    status = reader.GetString(reader.GetOrdinal("status")),
                    created_at = reader.GetDateTime(reader.GetOrdinal("created_at"))
                });
            }
        }
        catch (Exception)
        {
            // Return empty list if database connection fails
            return orders;
        }
        return orders;
    }

    public order? GetOrder(int id)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "SELECT [id], [table_id], [user_id], [status], [created_at] FROM [order] WHERE [id]=@id;";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new order
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    table_id = reader.GetInt32(reader.GetOrdinal("table_id")),
                    user_id = reader.GetInt32(reader.GetOrdinal("user_id")),
                    status = reader.GetString(reader.GetOrdinal("status")),
                    created_at = reader.GetDateTime(reader.GetOrdinal("created_at"))
                };
            }
        }
        catch (Exception)
        {
            // Return null if database connection fails
        }
        return null;
    }

    public List<order> GetOrdersByTableId(int tableId)
    {
        var orders = new List<order>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "SELECT [id], [table_id], [user_id], [status], [created_at] FROM [order] WHERE [table_id]=@table_id ORDER BY [created_at] DESC;";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@table_id", tableId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                orders.Add(new order
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    table_id = reader.GetInt32(reader.GetOrdinal("table_id")),
                    user_id = reader.GetInt32(reader.GetOrdinal("user_id")),
                    status = reader.GetString(reader.GetOrdinal("status")),
                    created_at = reader.GetDateTime(reader.GetOrdinal("created_at"))
                });
            }
        }
        catch (Exception)
        {
            // Return empty list if database connection fails
            return orders;
        }
        return orders;
    }

    public order? GetPendingOrderByTableId(int tableId)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "SELECT TOP 1 [id], [table_id], [user_id], [status], [created_at] FROM [order] WHERE [table_id]=@table_id AND [status]='Pending' ORDER BY [created_at] DESC;";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@table_id", tableId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new order
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    table_id = reader.GetInt32(reader.GetOrdinal("table_id")),
                    user_id = reader.GetInt32(reader.GetOrdinal("user_id")),
                    status = reader.GetString(reader.GetOrdinal("status")),
                    created_at = reader.GetDateTime(reader.GetOrdinal("created_at"))
                };
            }
        }
        catch (Exception)
        {
            // Return null if database connection fails
        }
        return null;
    }

    public List<order_item> GetOrderItems(int orderId)
    {
        var items = new List<order_item>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "SELECT [id], [order_id], [product_id], [qty], [price] FROM [order_item] WHERE [order_id]=@order_id;";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@order_id", orderId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new order_item
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    order_id = reader.GetInt32(reader.GetOrdinal("order_id")),
                    product_id = reader.GetInt32(reader.GetOrdinal("product_id")),
                    qty = reader.GetInt32(reader.GetOrdinal("qty")),
                    price = reader.GetDecimal(reader.GetOrdinal("price"))
                });
            }
        }
        catch (Exception)
        {
            // Return empty list if database connection fails
            return items;
        }
        return items;
    }

    public void UpdateOrderStatus(int orderId, string status)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            const string query = "UPDATE [order] SET [status]=@status WHERE [id]=@id;";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", orderId);
            cmd.Parameters.AddWithValue("@status", status);

            cmd.ExecuteNonQuery();
        }
        catch (Exception)
        {
            // Ignore error if database connection fails
        }
    }
}

