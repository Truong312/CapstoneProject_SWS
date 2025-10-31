using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.DTOs;
using SWS.BusinessObjects.Models;

namespace SWS.Repositories.Repositories.ReturnRepo
{
    public class ReturnStatusRepository : IReturnStatusRepository
    {
        private readonly SmartWarehouseDbContext _ctx;
        public ReturnStatusRepository(SmartWarehouseDbContext ctx) => _ctx = ctx;

        public async Task<List<ReturnStatusDto>> SearchAsync(string? q)
        {
            // baseQuery: chỉ còn chuỗi Status (đã loại null)
            var baseQuery = _ctx.ReturnOrders
                                .Where(ro => ro.Status != null);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                baseQuery = baseQuery.Where(ro => ro.Status!.Contains(q));
            }

            // GroupBy trên cột đơn giản (string) -> EF dịch tốt
            var grouped = await baseQuery
                .Select(ro => ro.Status!)              // chọn chuỗi
                .GroupBy(s => s)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .OrderBy(x => x.Status)
                .ToListAsync();                        // materialize

            // Map sang DTO ở phía client
            return grouped
                .Select(x => new ReturnStatusDto(x.Status, x.Count))
                .ToList();
        }
    }
}
