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
    }
}
