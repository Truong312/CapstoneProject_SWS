using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        //get all location that currently has the product with the productId
        public async Task<IEnumerable<Location>> GetByProductId(int productId)
        {
            var inventories = await _context.Inventories.Where(i => i.ProductId == productId).ToListAsync();
            var locations = new List<Location>();
            foreach (var inventory in inventories)
            {
                var location = GetSingleAsync(l => l.LocationId == inventory.LocationId);
                if (location.Result!=null)
                {
                    locations.Add(location.Result);
                }
            }
            return locations;
        }
    }
}
