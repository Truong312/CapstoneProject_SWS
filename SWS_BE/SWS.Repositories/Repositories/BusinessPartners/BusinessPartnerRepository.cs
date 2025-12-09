using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Repositories.BusinessPartners;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWS.Repositories.Repositories.BusinessPartners
{
    public class BusinessPartnerRepository : IBusinessPartnerRepository
    {
        private readonly SmartWarehouseDbContext _context;

        public BusinessPartnerRepository(SmartWarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BusinessPartner>> GetPartnersByTypeAsync(string partnerType)
        {
            return await _context.BusinessPartners
                .Where(p => p.Type == partnerType)
                .ToListAsync();
        }
    }
}
