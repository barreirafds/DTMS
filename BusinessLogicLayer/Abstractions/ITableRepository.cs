using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Abstractions;

public interface ITableRepository
{
    List<table> GetTables();
    table? GetTable(int id);
    void CreateTable(int number, int seats, string status);
    void UpdateTable(table table);
    void DeleteTable(int id);
}

