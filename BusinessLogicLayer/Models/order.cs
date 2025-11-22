using System;

namespace BusinessLogicLayer.Models;

public class order
{
    public int id { get; set; }
    public int table_id { get; set; }
    public int user_id { get; set; }
    public string status { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
}

