using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;

    public TableService(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public List<TableDTO> GetAllTables()
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

    public TableDTO? GetTableById(int id)
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

    public ValidationResult CreateTable(CreateTableDTO createTableDto)
    {
        // Validação: TableNumber precisa ser um número válido
        if (!int.TryParse(createTableDto.TableNumber, out var number))
        {
            return ValidationResult.Failure("Table Number needs to be a number.", nameof(createTableDto.TableNumber));
        }

        // Validação: número da mesa deve ser positivo
        if (number <= 0)
        {
            return ValidationResult.Failure("Table number must be greater than 0.", nameof(createTableDto.TableNumber));
        }

        // Validação: número de lugares deve ser positivo
        if (createTableDto.Seats <= 0)
        {
            return ValidationResult.Failure("Table seats must be greater than 0.", nameof(createTableDto.Seats));
        }

        // Validação: status não pode estar vazio
        if (string.IsNullOrWhiteSpace(createTableDto.Status))
        {
            return ValidationResult.Failure("Table status is required.", nameof(createTableDto.Status));
        }

        _tableRepository.CreateTable(number, createTableDto.Seats, createTableDto.Status);
        return ValidationResult.Success();
    }

    public ValidationResult UpdateTable(UpdateTableDTO updateTableDto)
    {
        // Validação: número da mesa deve ser positivo
        if (updateTableDto.Number <= 0)
        {
            return ValidationResult.Failure("The number of the table needs to be positive.", nameof(updateTableDto.Number));
        }

        // Validação: número de lugares deve ser positivo
        if (updateTableDto.Seats <= 0)
        {
            return ValidationResult.Failure("Table seats must be greater than 0.", nameof(updateTableDto.Seats));
        }

        // Validação: status não pode estar vazio
        if (string.IsNullOrWhiteSpace(updateTableDto.Status))
        {
            return ValidationResult.Failure("Table status is required.", nameof(updateTableDto.Status));
        }

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

    public void DeleteTable(int id)
    {
        _tableRepository.DeleteTable(id);
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

