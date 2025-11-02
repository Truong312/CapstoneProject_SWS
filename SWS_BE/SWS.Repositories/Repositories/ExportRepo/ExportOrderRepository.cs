using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.ExportRepo
{
    public class ExportOrderRepository : GenericRepository<ExportOrder>, IExportOrderRepository
    {
        private readonly SmartWarehouseDbContext _context;

        public ExportOrderRepository(SmartWarehouseDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExportOrder>> GetByCustomerId(int customerId)
        {
            return await _context.ExportOrders.Where(e => e.CustomerId == customerId).ToListAsync();
        }

        public async Task<IEnumerable<ExportOrder>> GetByStaff(int staffId)
        {
            return await _context.ExportOrders.Where(e => e.CreatedBy == staffId).ToListAsync();
        }

        public async Task<IEnumerable<ExportOrder>> GetShippedExportOrder()
        {
            return await _context.ExportOrders.Where(e => e.ShippedDate < DateOnly.FromDateTime(DateTime.Now)).ToListAsync();
        }

        public async Task<IEnumerable<ExportDetail>> GetExportDetails(int exportOrderId)
        {
            return await _context.ExportDetails.Where(e => e.ExportOrderId == exportOrderId).ToListAsync();
        }
    }
}
