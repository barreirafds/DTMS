using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs
{
    public class ProductDTO
    {
        public int id { get; set; }

        public string name { get; set; } = string.Empty;

        public string? description { get; set; }

        public decimal price { get; set; }

        public string category { get; set; } = string.Empty;

    }

    public class CreateProductDTO
    {
        public string name { get; set; } = string.Empty;
        public string? description { get; set; } = "";
        public decimal price { get; set; }
        public string category { get; set; } = string.Empty;
    }
}
