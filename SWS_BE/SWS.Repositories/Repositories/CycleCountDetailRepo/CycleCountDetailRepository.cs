using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.CycleCountDetailRepo
{
    public class CycleCountDetailRepository : GenericRepository<CycleCountDetail>, ICycleCountDetailRepository
    {
        private readonly SmartWarehouseDbContext _context;

        public CycleCountDetailRepository(SmartWarehouseDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CycleCountDetail>> GetAllByCycleCountId(int cycleCountId)
        {
            return await _context.CycleCountDetails.Where(c => c.CycleCountId == cycleCountId).ToListAsync();
        }

        public async Task<CycleCountDetail> GetByCycleCountIdAndProductId(int cycleCountId, int productId)
        {
            return await GetSingleAsync(c => c.CycleCountId == cycleCountId && c.ProductId == productId);
        }
        public async Task<CycleCountDetail> GetByCycleCountNameAndProductId(string cycleCountName, int productId)
        {
            var cycle = await _context.CycleCounts.FirstAsync(c => c.CycleName == cycleCountName);
            return await GetSingleAsync(c => c.CycleCountId == cycle.CycleCountId && c.ProductId == productId);
        }
    }
}
