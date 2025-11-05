using System;
using System.Collections.Generic;

namespace DataAcessLayer.Models;

public class product
{
    public int id { get; set; }

    public string name { get; set; } = string.Empty;

    public string? description { get; set; }

    public decimal price { get; set; }

    public string category { get; set; } = string.Empty;

    public DateTime? created_at { get; set; }
}

