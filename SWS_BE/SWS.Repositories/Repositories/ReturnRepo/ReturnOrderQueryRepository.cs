using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Models;

namespace SWS.Repositories.Repositories.ReturnRepo
{
    public class ReturnOrderQueryRepository : IReturnOrderQueryRepository
    {
        private readonly SmartWarehouseDbContext _ctx;
        public ReturnOrderQueryRepository(SmartWarehouseDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<ReturnOrder>> GetListAsync(
            DateOnly? from, DateOnly? to, string? status,
            int? exportOrderId, int? checkedBy, int? reviewedBy)
        {
            var query = _ctx.ReturnOrders
                .Include(ro => ro.CheckedByNavigation)
                .Include(ro => ro.ReviewedByNavigation)
                .AsQueryable();

            if (from.HasValue)
            {
                var fromDt = from.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(ro => ro.CheckInTime == null || ro.CheckInTime >= fromDt);
            }
            if (to.HasValue)
            {
                var toDt = to.Value.ToDateTime(TimeOnly.MaxValue);
                query = query.Where(ro => ro.CheckInTime == null || ro.CheckInTime <= toDt);
            }
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(ro => ro.Status == status);

            if (exportOrderId.HasValue)
                query = query.Where(ro => ro.ExportOrderId == exportOrderId.Value);

            if (checkedBy.HasValue)
                query = query.Where(ro => ro.CheckedBy == checkedBy.Value);

            if (reviewedBy.HasValue)
                query = query.Where(ro => ro.ReviewedBy == reviewedBy.Value);

            return await query
                .OrderByDescending(ro => ro.CheckInTime)
                .ToListAsync();
        }

        public async Task<ReturnOrder?> GetDetailAsync(int id)
        {
            return await _ctx.ReturnOrders
                .Include(ro => ro.CheckedByNavigation)
                .Include(ro => ro.ReviewedByNavigation)
                .Include(ro => ro.ReturnOrderDetails)
                    .ThenInclude(d => d.Product)
                .Include(ro => ro.ReturnOrderDetails)
                    .ThenInclude(d => d.Reason)
                .Include(ro => ro.ReturnOrderDetails)
                    .ThenInclude(d => d.Action)
                .Include(ro => ro.ReturnOrderDetails)
                    .ThenInclude(d => d.Location)
                .FirstOrDefaultAsync(ro => ro.ReturnOrderId == id);
        }
    }
}
