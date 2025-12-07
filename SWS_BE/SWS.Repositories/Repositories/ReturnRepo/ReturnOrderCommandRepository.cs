using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Constants;
using SWS.BusinessObjects.DTOs;
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

        //  CREATE RETURN ORDER
        public async Task<CreateReturnOrderResponse> CreateAsync(
            int createdBy,
            CreateReturnOrderRequest req,
            CancellationToken ct = default)
        {
            if (req.Items == null || !req.Items.Any())
                throw new InvalidOperationException("Return order must have at least one item.");

            // Validate product
            var productIds = req.Items.Select(i => i.ProductId).Distinct().ToList();
            var validProducts = await _ctx.Products
                .Where(p => productIds.Contains(p.ProductId))
                .Select(p => p.ProductId)
                .ToListAsync(ct);

            var missingProducts = productIds.Except(validProducts).ToList();
            if (missingProducts.Count > 0)
                throw new InvalidOperationException(
                    $"ProductId không hợp lệ: {string.Join(", ", missingProducts)}");

            // (Optional) validate ExportOrderId nếu có
            if (req.ExportOrderId.HasValue)
            {
                var exportExists = await _ctx.ExportOrders
                    .AnyAsync(e => e.ExportOrderId == req.ExportOrderId.Value, ct);
                if (!exportExists)
                    throw new InvalidOperationException("Export order không tồn tại.");
            }

            // (Optional) có thể validate ReasonId, ActionId, LocationId nếu muốn chặt hơn

            foreach (var item in req.Items)
            {
                if (item.Quantity <= 0)
                    throw new InvalidOperationException("Quantity phải > 0");
            }

            var now = DateTime.UtcNow;

            var order = new ReturnOrder
            {
                ExportOrderId = req.ExportOrderId,
                CheckedBy = null,
                ReviewedBy = null,
                CheckInTime = now,
                Status = ReturnStatuses.Pending,  // dùng constant bạn đã có
                Note = req.Note
            };

            // gắn details qua navigation
            foreach (var item in req.Items)
            {
                order.ReturnOrderDetails.Add(new ReturnOrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Note = item.Note,
                    ReasonId = item.ReasonId,
                    ActionId = item.ActionId,
                    LocationId = item.LocationId
                });
            }

            await _ctx.ReturnOrders.AddAsync(order, ct);

            await _ctx.ActionLogs.AddAsync(new ActionLog
            {
                UserId = createdBy,
                ActionType = "RETURN_CREATED",
                EntityType = "ReturnOrder",
                Timestamp = now,
                Description = $"ReturnOrder #{order.ReturnOrderId} created. Note: {req.Note}"
            }, ct);

            await _ctx.SaveChangesAsync(ct);

            return new CreateReturnOrderResponse(order.ReturnOrderId);
        }
    }
}
