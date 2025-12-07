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

        public async Task<IEnumerable<Location>> GetByProductId(int productId)
        {
            var inventories = await _context.Inventories.Where(i => i.ProductId == productId).ToListAsync();
            var locations = new List<Location>();
            foreach(var inventory in inventories)
            {
                var location = await GetSingleAsync(l => l.LocationId == inventory.LocationId);
                if(location!=null) locations.Add(location);
            }
            return locations;
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
        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
       => await _context.Locations.Include(x => x.Inventories).ThenInclude(p => p.Product).ToListAsync();

        public async Task<Location?> GetLocationByIdAsync(int id)
            => await _context.Locations.Include(x => x.Inventories).ThenInclude(p => p.Product)
                                       .FirstOrDefaultAsync(x => x.LocationId == id);

        public async Task<IEnumerable<Inventory>> GetProductLocationsAsync(int productId)
            => await _context.Inventories.Include(l => l.Location)
                                         .Include(p => p.Product)
                                         .Where(x => x.ProductId == productId)
                                         .ToListAsync();

        public async Task AddInventoryAsync(Inventory inventory)
            => await _context.Inventories.AddAsync(inventory);

        public async Task<bool> UpdateInventoryQuantityAsync(int inventoryId, int quantity)
        {
            var item = await _context.Inventories.FindAsync(inventoryId);
            if (item == null) return false;
            item.QuantityAvailable = quantity;
            return true;
        }

        public async Task<bool> RemoveInventoryAsync(int inventoryId)
        {
            var inv = await _context.Inventories.FindAsync(inventoryId);
            if (inv == null) return false;
            _context.Inventories.Remove(inv);
            return true;
        }
    }
}
