using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.TransactionLogRepo
{
    public class TransactionLogRepository : GenericRepository<TransactionLog>, ITransactionLogRepository
    {
        private readonly SmartWarehouseDbContext _context;
        public TransactionLogRepository(SmartWarehouseDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionLog>> GetWithCondition(DateOnly? startDate, DateOnly? endDate, TransactionType? transactionType)
        {
            var now = DateOnly.FromDateTime(DateTime.Now);
            if (startDate == null)
            {
                startDate = DateOnly.MinValue;
            }
            if (endDate == null)
            {
                endDate = now;
            }
            var transactionLog = _context.TransactionLogs.Where(t =>
            t.TransactionDate >= new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day)
            && t.TransactionDate <= new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day));
            if (transactionType != null)
            {
                return await transactionLog.Where(t => t.Type == transactionType.Value.ToString()).ToListAsync();
            }
            else
            {
                return await transactionLog.ToListAsync();//return transactionLogs with all Type
            }
        }
    }
}
