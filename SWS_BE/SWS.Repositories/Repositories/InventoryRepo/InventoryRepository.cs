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


    }
}
