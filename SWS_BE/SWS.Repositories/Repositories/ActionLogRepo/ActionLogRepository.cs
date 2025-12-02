using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.ActionLogRepo
{
    public class ActionLogRepository: GenericRepository<ActionLog>, IActionLogRepository
    {
        private readonly SmartWarehouseDbContext _context;

        public ActionLogRepository(SmartWarehouseDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActionLog>> GetWithCondition(DateOnly? startDate, DateOnly? endDate, ActionType? actionType)
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
            var actionlog = _context.ActionLogs.Where(a =>
            a.Timestamp >= new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day)
            && a.Timestamp <= new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day));
            if (actionType != null)
            {
                return await actionlog.Where(a => a.ActionType == actionType.Value.ToString()).ToListAsync();
            }
            else
            {
                return await actionlog.ToListAsync();//return ActionLogs with all ActionType
            }

        }
    }
}
