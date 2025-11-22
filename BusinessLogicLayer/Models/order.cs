using System;
using System.Collections.Generic;

namespace BusinessLogicLayer.Models;

public class order
{
    public int id { get; set; }

    public int table_id { get; set; }

    public DateTime created_at { get; set; }

    public List<order_item> items { get; set; } = new();
}
