using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.LocationRepo
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {
        private readonly SmartWarehouseDbContext _context;

        public LocationRepository(SmartWarehouseDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Location>> GetLayoutAsync(string? shelfId, int? productId)
        {
            var query = _context.Locations
                .Include(l => l.Inventories)
                    .ThenInclude(inv => inv.Product)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(shelfId))
            {
                query = query.Where(l => l.ShelfId != null && l.ShelfId == shelfId);
            }

            if (productId.HasValue)
            {
                query = query.Where(l => l.Inventories
                    .Any(inv => inv.ProductId == productId.Value));
            }

            return await query
                .OrderBy(l => l.ShelfId)
                .ThenBy(l => l.RowNumber)
                .ThenBy(l => l.ColumnNumber)
                .ToListAsync();
        }
    }
}
