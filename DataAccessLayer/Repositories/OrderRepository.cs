using System;
using System.Collections.Generic;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Models;
using Microsoft.Data.SqlClient;

namespace DataAcessLayer.Repositories;

public class OrderRepository : IOrderRepository
{
    public string connString = "Server=mssqlstud.fhict.local;Database=dbi570286_dbdtms1;User Id=dbi570286_dbdtms1;Password=Root1234;TrustServerCertificate=True;";

    public int CreateOrderWithItems(order order, List<order_item> orderItems)
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

    public int CreateOrder(order order)
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

    public void CreateOrderItem(order_item orderItem)
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

    public List<order> GetOrders()
    {
        var orders = new List<order>();
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
        return orders;
    }

    public order? GetOrder(int id)
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
        return null;
    }

    public List<order> GetOrdersByTableId(int tableId)
    {
        var orders = new List<order>();
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
        return orders;
    }

    public order? GetPendingOrderByTableId(int tableId)
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
        return null;
    }

    public List<order_item> GetOrderItems(int orderId)
    {
        var items = new List<order_item>();
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
        return items;
    }

    public void UpdateOrderStatus(int orderId, string status)
    {
        using var conn = new SqlConnection(connString);
        conn.Open();

        const string query = "UPDATE [order] SET [status]=@status WHERE [id]=@id;";
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", orderId);
        cmd.Parameters.AddWithValue("@status", status);

        cmd.ExecuteNonQuery();
    }

    public int GetOrderCountByUserId(int userId)
    {
        using var conn = new SqlConnection(connString);
        conn.Open();

        const string query = "SELECT COUNT(*) FROM [order] WHERE [user_id]=@user_id;";
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@user_id", userId);
        
        var count = (int)cmd.ExecuteScalar();
        return count;
    }

    public void DeleteOrdersByTableId(int tableId)
    {
        using var conn = new SqlConnection(connString);
        conn.Open();
        
        using var transaction = conn.BeginTransaction();
        try
        {
            // First, get all order IDs for this table
            var orderIds = new List<int>();
            const string getOrdersQuery = "SELECT [id] FROM [order] WHERE [table_id]=@table_id;";
            using (var getOrdersCmd = new SqlCommand(getOrdersQuery, conn, transaction))
            {
                getOrdersCmd.Parameters.AddWithValue("@table_id", tableId);
                using var reader = getOrdersCmd.ExecuteReader();
                while (reader.Read())
                {
                    orderIds.Add(reader.GetInt32(reader.GetOrdinal("id")));
                }
            }

            // Delete all order_items for these orders
            if (orderIds.Count > 0)
            {
                const string deleteItemsQuery = "DELETE FROM [order_item] WHERE [order_id]=@order_id;";
                foreach (var orderId in orderIds)
                {
                    using var deleteItemsCmd = new SqlCommand(deleteItemsQuery, conn, transaction);
                    deleteItemsCmd.Parameters.AddWithValue("@order_id", orderId);
                    deleteItemsCmd.ExecuteNonQuery();
                }
            }

            // Finally, delete all orders for this table
            const string deleteOrdersQuery = "DELETE FROM [order] WHERE [table_id]=@table_id;";
            using var deleteOrdersCmd = new SqlCommand(deleteOrdersQuery, conn, transaction);
            deleteOrdersCmd.Parameters.AddWithValue("@table_id", tableId);
            deleteOrdersCmd.ExecuteNonQuery();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public List<order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
    {
        var orders = new List<order>();
        using var conn = new SqlConnection(connString);
        conn.Open();

        // Set endDate to end of day (23:59:59) to include the entire day
        var endDateWithTime = endDate.Date.AddDays(1).AddTicks(-1);

        const string query = "SELECT [id], [table_id], [user_id], [status], [created_at] FROM [order] WHERE [created_at] >= @start_date AND [created_at] <= @end_date ORDER BY [created_at] DESC;";
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@start_date", startDate.Date);
        cmd.Parameters.AddWithValue("@end_date", endDateWithTime);

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
        return orders;
    }
}

