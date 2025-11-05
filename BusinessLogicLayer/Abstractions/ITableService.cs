using DataAcessLayer.Models;

namespace BusinessLogicLayer.Abstractions;

public interface ITableService
{
    List<table> GetAllTables();
    table? GetTableById(int id);
    void CreateTable(int number, int seats, string status);
    void UpdateTable(table table);
    void DeleteTable(int id);
    string GetStatusBadgeStyle(string status);
}

