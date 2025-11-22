using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Dtos;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SWS.Repositories.Repositories.InventoryRepo
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly SmartWarehouseDbContext _db;

        public InventoryRepository(SmartWarehouseDbContext db)
        {
            _db = db;
        }

        public async Task<Inventory?> GetInventoryAsync(int productId, int locationId)
        {
            return await _db.Inventories
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.LocationId == locationId);
        }

        public async Task<List<Inventory>> GetInventoryByProductAsync(int productId)
        {
            return await _db.Inventories
                .Where(x => x.ProductId == productId)
                .ToListAsync();
        }

        public async Task<InventoryStatusSummaryDto> GetStockByStatusAsync()
        {
            var grouped = await _db.Inventories
                .GroupBy(i => i.Status)
                .Select(g => new { Status = g.Key, Total = g.Sum(x => x.QuantityAvailable) })
                .ToListAsync();

            return new InventoryStatusSummaryDto(
                Available: grouped.FirstOrDefault(x => x.Status == InventoryStatus.Available)?.Total ?? 0,
                Allocated: grouped.FirstOrDefault(x => x.Status == InventoryStatus.Allocated)?.Total ?? 0,
                Damaged: grouped.FirstOrDefault(x => x.Status == InventoryStatus.Damaged)?.Total ?? 0,
                InTransit: grouped.FirstOrDefault(x => x.Status == InventoryStatus.InTransit)?.Total ?? 0
            );
        }

        public async Task<List<ProductInventoryDto>> GetAllProductInventoryAsync()
        {
            var result = await _db.Products
         .Select(p => new ProductInventoryDto(
             p.ProductId,
             p.Name,
             _db.Inventories.Where(i => i.ProductId == p.ProductId).Sum(i => i.QuantityAvailable),
             _db.Inventories.Where(i => i.ProductId == p.ProductId && i.Status == InventoryStatus.Available).Sum(i => i.QuantityAvailable),
             _db.Inventories.Where(i => i.ProductId == p.ProductId && i.Status == InventoryStatus.Allocated).Sum(i => i.QuantityAvailable),
             _db.Inventories.Where(i => i.ProductId == p.ProductId && i.Status == InventoryStatus.Damaged).Sum(i => i.QuantityAvailable),
             _db.Inventories.Where(i => i.ProductId == p.ProductId && i.Status == InventoryStatus.InTransit).Sum(i => i.QuantityAvailable)
         ))
         .ToListAsync();

            return result;
        }
        //public async Task<List<ProductInventoryDto>> GetProductInventoryByLocationAsync(int locationId)
        //{
        //    return await _db.Inventories
        //        .Where(x => x.LocationId == locationId)
        //        .GroupBy(x => x.Product)
        //        .Select(g => new ProductInventoryDto(
        //            g.Key.ProductId,
        //            g.Key.Name,
        //            g.Sum(x => x.QuantityAvailable)
        //        ))
        //        .ToListAsync();
        //}


    }
}
