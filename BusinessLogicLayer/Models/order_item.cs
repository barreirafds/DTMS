namespace BusinessLogicLayer.Models;

public class order_item
{
    public int id { get; set; }
    public int order_id { get; set; }
    public int product_id { get; set; }
    public int qty { get; set; }
    public decimal price { get; set; }
}

