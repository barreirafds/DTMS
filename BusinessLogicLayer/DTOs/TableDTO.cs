namespace BusinessLogicLayer.DTOs;

public class TableDTO
{
    public int Id { get; set; }
    public int Number { get; set; }
    public int Seats { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreateTableDTO
{
    public string TableNumber { get; set; } = string.Empty;
    public int Seats { get; set; }
    public string Status { get; set; } = "Available";
}

public class UpdateTableDTO
{
    public int Id { get; set; }
    public int Number { get; set; }
    public int Seats { get; set; }
    public string Status { get; set; } = string.Empty;
}

