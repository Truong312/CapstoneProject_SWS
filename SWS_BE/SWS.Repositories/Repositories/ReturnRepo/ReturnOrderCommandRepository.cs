// File: SWS.Repositories/Repositories/ReturnRepo/ReturnOrderCommandRepository.cs
using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Models;

namespace SWS.Repositories.Repositories.ReturnRepo
{
    public class ReturnOrderCommandRepository : IReturnOrderCommandRepository
    {
        private readonly SmartWarehouseDbContext _ctx;
        public ReturnOrderCommandRepository(SmartWarehouseDbContext ctx) => _ctx = ctx;

        public Task<ReturnOrder?> GetForUpdateAsync(int id, CancellationToken ct = default)
            => _ctx.ReturnOrders
                .Include(r => r.CheckedByNavigation)
                .Include(r => r.ReviewedByNavigation)
                .Include(r => r.ReturnOrderDetails).ThenInclude(d => d.Product)
                .Include(r => r.ReturnOrderDetails).ThenInclude(d => d.Reason)
                .Include(r => r.ReturnOrderDetails).ThenInclude(d => d.Action)
                .Include(r => r.ReturnOrderDetails).ThenInclude(d => d.Location)
                .FirstOrDefaultAsync(r => r.ReturnOrderId == id, ct);

        public async Task AddActionLogAsync(ActionLog log, CancellationToken ct = default)
            => await _ctx.ActionLogs.AddAsync(log, ct);
    }
}
