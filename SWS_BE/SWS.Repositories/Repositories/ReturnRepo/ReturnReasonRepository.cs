using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Models;
using SWS.BusinessObjects.Models; // DbContext
namespace SWS.Repositories.Repositories.ReturnRepo
{
    public class ReturnReasonRepository : IReturnReasonRepository
    {
        private readonly SmartWarehouseDbContext _ctx;
        public ReturnReasonRepository(SmartWarehouseDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<ReturnReason>> SearchAsync(string? q)
        {
            var query = _ctx.ReturnReasons.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(r =>
                    (r.ReasonCode != null && r.ReasonCode.Contains(q)) ||
                    (r.Description != null && r.Description.Contains(q)));
            }
            return await query.OrderBy(r => r.ReasonCode).ToListAsync();
        }
    }
}
