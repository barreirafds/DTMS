using BusinessLogicLayer.Abstractions;
using DataAcessLayer;
using DataAcessLayer.Models;

namespace BusinessLogicLayer.Services;

public class TableService : ITableService
{
    private readonly tableconn _tableConn;

    public TableService()
    {
        _tableConn = new tableconn();
    }

    public List<table> GetAllTables()
    {
        return _tableConn.GetTables();
    }

    public table? GetTableById(int id)
    {
        return _tableConn.GetTable(id);
    }

    public void CreateTable(int number, int seats, string status)
    {
        _tableConn.CreateTable(number, seats, status);
    }

    public void UpdateTable(table table)
    {
        _tableConn.UpdateTable(table);
    }

    public void DeleteTable(int id)
    {
        _tableConn.DeleteTable(id);
    }

    public string GetStatusBadgeStyle(string status)
    {
        var s = (status ?? "Available").ToLowerInvariant();
        string bg = "#e2e3e5"; 
        string fg = "#383d41";
        
        if (s == "available") { bg = "#d4edda"; fg = "#155724"; }
        else if (s == "occupied") { bg = "#f8d7da"; fg = "#721c24"; }
        else if (s == "reserved") { bg = "#fff3cd"; fg = "#856404"; }
        else if (s == "outofservice") { bg = "#e2e3e5"; fg = "#383d41"; }

        return $"display:inline-block; margin-left:8px; padding:2px 8px; border-radius:12px; font-size:12px; background:{bg}; color:{fg};";
    }
}

