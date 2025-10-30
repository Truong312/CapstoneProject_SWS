using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.DTOs;
using SWS.BusinessObjects.Models; // giữ 1 using thôi

namespace SWS.Repositories.Repositories.ReturnRepo
{
    public class ReturnStatusRepository : IReturnStatusRepository
    {
        private readonly SmartWarehouseDbContext _ctx;
        public ReturnStatusRepository(SmartWarehouseDbContext ctx) => _ctx = ctx;

        public async Task<List<ReturnStatusDto>> SearchAsync(string? q)
        {
            var baseQuery = _ctx.ReturnOrders.Where(ro => ro.Status != null);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                baseQuery = baseQuery.Where(ro => ro.Status!.Contains(q));
            }

            return await baseQuery
                .GroupBy(ro => ro.Status!)                 // đã loại null ở trên
                .Select(g => new ReturnStatusDto(g.Key, g.Count())) // dùng positional args
                .OrderBy(x => x.Status)
                .ToListAsync();
        }
    }
}
