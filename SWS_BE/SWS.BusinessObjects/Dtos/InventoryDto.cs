using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.BusinessObjects.Dtos
{
    public sealed record InventoryDto(
     int InventoryId,
     int ProductId,
     int LocationId,
     int QuantityAvailable,
     int AllocatedQuantity
 );
    public sealed record InventoryAdjustmentDto(
        int ProductId,
        int LocationId,
        int NewQuantity,
        string Reason
    );
    public sealed record InventoryTransferDto(
        int ProductId,
        int FromLocationId,
        int ToLocationId,
        int Quantity
    );
    public sealed class InventoryDashboardDto
    {
        public decimal TotalStockValue { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public decimal InventoryTurnoverRate { get; set; }
    }
    public sealed record InventoryStatusSummaryDto(
        int Available,
        int Allocated,
        int Damaged,
        int InTransit
);
    public sealed record ProductInventoryDto(
        int ProductId,
        string ProductName,
        int TotalStock,
        int Available ,
        int Allocated ,
        int Damaged ,
        int InTransit 
    );
    public sealed record LowStockItemDto(
        int ProductId,
        string ProductName,
        int TotalStock
);

}
