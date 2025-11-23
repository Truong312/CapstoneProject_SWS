using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Repositories.Repositories.InventoryRepo
{
    public class InventoryDashboardRepository : IInventoryDashboardRepository
    {
        private readonly SmartWarehouseDbContext _db;

        public InventoryDashboardRepository(SmartWarehouseDbContext db)
        {
            _db = db;
        }

        public async Task<decimal> GetTotalStockValueAsync()
        {
            return (await _db.Inventories
                .Join(_db.Products,
                    inv => inv.ProductId,
                    prod => prod.ProductId,
                    (inv, prod) => new { inv.QuantityAvailable, prod.PurchasedPrice })
                .Select(x => (decimal?)x.QuantityAvailable * x.PurchasedPrice)
                .SumAsync()) ?? 0m; // Trả về 0 nếu giá trị là null
        }

        public async Task<int> GetLowStockCountAsync()
        {
            return await _db.Products
                .Join(_db.Inventories,
                    p => p.ProductId,
                    i => i.ProductId,
                    (p, i) => new { p, i })
                .Where(x => x.i.QuantityAvailable < x.p.ReorderPoint)
                .Select(x => x.p.ProductId)
                .Distinct()
                .CountAsync();
        }

        public async Task<int> GetOutOfStockCountAsync()
        {
            return await _db.Inventories
                .GroupBy(x => x.ProductId)
                .Where(g => g.Sum(x => x.QuantityAvailable) == 0)
                .CountAsync();
        }

        public async Task<decimal> GetInventoryTurnoverRateAsync()
        {
            // Example formula: Cost of Goods Sold / Average Inventory Value (per month)
            decimal totalValue = await GetTotalStockValueAsync();

            // This formula can be replaced based on your business rules:
            decimal cogsLastMonth = await _db.TransactionLogs
                .Where(t => t.Type == "EXPORT"
                    && t.TransactionDate >= DateTime.UtcNow.AddMonths(-1))
                .SumAsync(t => (decimal?)t.Quantity * t.Product.PurchasedPrice) ?? 0m;

            if (totalValue == 0) return 0;

            return Math.Round(cogsLastMonth / totalValue, 2);
        }
    }

}
