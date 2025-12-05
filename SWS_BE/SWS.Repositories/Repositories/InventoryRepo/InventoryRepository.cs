using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Dtos;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.InventoryRepo
{
    public class InventoryRepository: GenericRepository<Inventory>, IInventoryRepository
    {
        private readonly SmartWarehouseDbContext _context;

        public InventoryRepository(SmartWarehouseDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Inventory> GetByProductId(int productId)
        {
            return await GetSingleAsync(i=>i.ProductId==productId);
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

        //dùng đề update quantity khi tạo export, import
        public async Task UpdateQuantity(int productId, int quantity)
        {
            var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);
            if (inventory == null)
            {
                throw new Exception($"Không tìm thấy sản phẩm với ID = {productId}");
            }
            if (inventory.QuantityAvailable < -quantity)//example quantity=20 export 30 => quantity = 20 +(-30)= -10 or 20< -(-30)=> throw
            {
                throw new Exception($"sản phẩm hiện đang không đủ");
            }
            inventory.QuantityAvailable += quantity;
            await _context.SaveChangesAsync();
        }
    }
}
