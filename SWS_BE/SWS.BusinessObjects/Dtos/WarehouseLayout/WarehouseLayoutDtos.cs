using System.Collections.Generic;
using System.Linq;

namespace SWS.BusinessObjects.Dtos.WarehouseLayout
{
    public class WarehouseCellProductDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int QuantityAvailable { get; set; }
    }

    public class WarehouseCellDto
    {
        public int LocationId { get; set; }
        public string? ShelfId { get; set; }
        public int? RowNumber { get; set; }
        public int? ColumnNumber { get; set; }
        public string? Type { get; set; }
        public bool IsFull { get; set; }

        // FE dùng để tô đậm ô có hàng
        public bool HasProduct => Products.Any();

        // Tổng số lượng ở ô này
        public int TotalQuantity => Products.Sum(p => p.QuantityAvailable);

        public List<WarehouseCellProductDto> Products { get; set; } = new();
    }

    public class WarehouseShelfDto
    {
        public string? ShelfId { get; set; }
        public int MaxRow { get; set; }
        public int MaxColumn { get; set; }
        public List<WarehouseCellDto> Cells { get; set; } = new();
    }

    public class WarehouseLayoutResponse
    {
        public List<WarehouseShelfDto> Shelves { get; set; } = new();
    }
}
