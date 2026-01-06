using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Abstractions;

public interface ITableService
{
    List<TableDTO> GetAllTables();
    TableDTO? GetTableById(int id);
    ValidationResult CreateTable(CreateTableDTO createTableDto);
    ValidationResult UpdateTable(UpdateTableDTO updateTableDto);
    ValidationResult DeleteTable(int id);
    string GetStatusBadgeStyle(string status);
}

