using System;

namespace BusinessLogicLayer.Models;

public class order_item
{
    public int id { get; set; }

    public int order_id { get; set; }

    public int product_id { get; set; }

    public int quantity { get; set; }

    public decimal unit_price { get; set; }

    public product? product { get; set; }
}
