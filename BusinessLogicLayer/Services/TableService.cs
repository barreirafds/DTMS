using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;
    private readonly IOrderRepository _orderRepository;

    public TableService(ITableRepository tableRepository, IOrderRepository orderRepository)
    {
        _tableRepository = tableRepository;
        _orderRepository = orderRepository;
    }

    public List<TableDTO> GetAllTables()
    {
        try
        {
            var tables = _tableRepository.GetTables();
            return tables.Select(t => new TableDTO
            {
                Id = t.id,
                Number = t.number,
                Seats = t.seats,
                Status = t.status
            }).ToList();
        }
        catch (Exception)
        {
            return new List<TableDTO>();
        }
    }

    public TableDTO? GetTableById(int id)
    {
        try
        {
            var table = _tableRepository.GetTable(id);
            if (table == null) return null;

            return new TableDTO
            {
                Id = table.id,
                Number = table.number,
                Seats = table.seats,
                Status = table.status
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public ValidationResult CreateTable(CreateTableDTO createTableDto)
    {
        if (!int.TryParse(createTableDto.TableNumber, out var number))
        {
            return ValidationResult.Failure("Table Number needs to be a number.", nameof(createTableDto.TableNumber));
        }

        if (number <= 0)
        {
            return ValidationResult.Failure("Table number must be greater than 0.", nameof(createTableDto.TableNumber));
        }

        if (createTableDto.Seats <= 0)
        {
            return ValidationResult.Failure("Table seats must be greater than 0.", nameof(createTableDto.Seats));
        }

        if (string.IsNullOrWhiteSpace(createTableDto.Status))
        {
            return ValidationResult.Failure("Table status is required.", nameof(createTableDto.Status));
        }

        try
        {
            _tableRepository.CreateTable(number, createTableDto.Seats, createTableDto.Status);
            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            return ValidationResult.Failure($"Error creating table: {ex.Message}");
        }
    }

    public ValidationResult UpdateTable(UpdateTableDTO updateTableDto)
    {
        if (updateTableDto.Number <= 0)
        {
            return ValidationResult.Failure("The number of the table needs to be positive.", nameof(updateTableDto.Number));
        }

        if (updateTableDto.Seats <= 0)
        {
            return ValidationResult.Failure("Table seats must be greater than 0.", nameof(updateTableDto.Seats));
        }

        if (string.IsNullOrWhiteSpace(updateTableDto.Status))
        {
            return ValidationResult.Failure("Table status is required.", nameof(updateTableDto.Status));
        }

        try
        {
            var table = new table
            {
                id = updateTableDto.Id,
                number = updateTableDto.Number,
                seats = updateTableDto.Seats,
                status = updateTableDto.Status
            };

            _tableRepository.UpdateTable(table);
            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            return ValidationResult.Failure($"Error updating table: {ex.Message}");
        }
    }

    public ValidationResult DeleteTable(int id)
    {
        try
        {
            // Check if table exists
            var table = _tableRepository.GetTable(id);
            if (table == null)
            {
                return ValidationResult.Failure("Table not found.");
            }

            // Delete all related orders and order items first
            _orderRepository.DeleteOrdersByTableId(id);

            // Now delete the table
            _tableRepository.DeleteTable(id);
            
            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            return ValidationResult.Failure($"Error deleting table: {ex.Message}");
        }
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

