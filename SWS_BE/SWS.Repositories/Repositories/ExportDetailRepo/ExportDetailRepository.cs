using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.ExportDetailRepo
{
    public class ExportDetailRepository: GenericRepository<ExportDetail>, IExportDetailRepository
    {
        private readonly SmartWarehouseDbContext _context;

        public ExportDetailRepository(SmartWarehouseDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExportDetail>> GetAllByExportOrderId(int exportOrderId)
        {
            return await _context.ExportDetails.Where(e => e.ExportOrderId == exportOrderId).ToListAsync();
        }
    }
}
