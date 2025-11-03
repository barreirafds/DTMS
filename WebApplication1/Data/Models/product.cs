using System;
using System.Collections.Generic;

namespace DTMS.Data.Models;

public partial class product
{
    public int id { get; set; }

    public string name { get; set; } = string.Empty;

    public string? description { get; set; }

    public decimal price { get; set; }

    public string category { get; set; } = string.Empty;

    public DateTime? created_at { get; set; }
}

