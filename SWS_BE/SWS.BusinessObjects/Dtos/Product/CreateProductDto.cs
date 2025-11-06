using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.BusinessObjects.Dtos.Product
{
    public class CreateProductDto
    {
        public string SerialNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateOnly ExpiredDate { get; set; }
        public string? Unit { get; set; }
        public decimal? UnitPrice { get; set; }
        public DateOnly ReceivedDate { get; set; }
        public decimal? PurchasedPrice { get; set; }
        public int? ReorderPoint { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
    }
}
