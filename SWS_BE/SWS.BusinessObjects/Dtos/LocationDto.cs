using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.BusinessObjects.Dtos
{
    public class LocationDetailDto
    {
        public int LocationId { get; set; }
        public string? ShelfId { get; set; }
        public int? Column { get; set; }
        public int? Row { get; set; }
        public string? Type { get; set; }
        public bool? IsFull { get; set; }

        public List<ProductInShelfDto> Products { get; set; } = new();
    }

    public class ProductInShelfDto
    {
        public int ProductId { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public int Available { get; set; }
    }

}
