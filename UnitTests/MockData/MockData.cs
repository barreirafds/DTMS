using BusinessLogicLayer.Models;

namespace UnitTests.MockData;

public static class MockData
{
    // Mock Tables
    public static List<table> GetMockTables()
    {
        return new List<table>
        {
            new table
            {
                id = 1,
                number = 1,
                seats = 4,
                status = "Available"
            },
            new table
            {
                id = 2,
                number = 2,
                seats = 6,
                status = "Occupied"
            }
        };
    }

    // Mock Products
    public static List<product> GetMockProducts()
    {
        return new List<product>
        {
            // Products for Table 1
            new product
            {
                id = 1,
                name = "Pizza Margherita",
                description = "Traditional pizza with tomato and mozzarella",
                price = 12.50m,
                category = "Food",
                created_at = DateTime.Now
            },
            new product
            {
                id = 2,
                name = "Coca-Cola",
                description = "Soft drink 330ml",
                price = 2.50m,
                category = "Beverage",
                created_at = DateTime.Now
            },
            // Products for Table 2
            new product
            {
                id = 3,
                name = "Classic Burger",
                description = "Burger with cheese and fries",
                price = 8.90m,
                category = "Food",
                created_at = DateTime.Now
            },
            new product
            {
                id = 4,
                name = "Water",
                description = "Mineral water 500ml",
                price = 1.50m,
                category = "Beverage",
                created_at = DateTime.Now
            }
        };
    }

    // Mock Users
    public static List<user> GetMockUsers()
    {
        return new List<user>
        {
            new user
            {
                id = 1,
                user1 = "admin",
                password = "admin123",
                role = "Admin"
            },
            new user
            {
                id = 2,
                user1 = "waiter",
                password = "waiter123",
                role = "Waiter"
            }
        };
    }
}

