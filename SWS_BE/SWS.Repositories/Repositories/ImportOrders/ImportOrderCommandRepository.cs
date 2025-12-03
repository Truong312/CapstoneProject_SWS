using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.DTOs;
using SWS.BusinessObjects.Models;

namespace SWS.Repositories.Repositories.ImportOrders;

public class ImportOrderCommandRepository : IImportOrderCommandRepository
{
    private readonly SmartWarehouseDbContext _ctx;
    public ImportOrderCommandRepository(SmartWarehouseDbContext ctx) => _ctx = ctx;

    public async Task<CreateImportOrderResponse> CreateAsync(
        int createdBy,
        CreateImportOrderRequest req,
        CancellationToken ct = default)
    {
        // Validate Provider
        var provider = await _ctx.BusinessPartners
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PartnerId == req.ProviderId, ct);
        if (provider == null) throw new InvalidOperationException("Provider không tồn tại.");

        // Validate Products
        var productIds = req.Items.Select(i => i.ProductId).Distinct().ToList();
        var validProducts = await _ctx.Products
            .Where(p => productIds.Contains(p.ProductId))
            .Select(p => p.ProductId)
            .ToListAsync(ct);
        var missing = productIds.Except(validProducts).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException($"ProductId không hợp lệ: {string.Join(", ", missing)}");

        // Chuẩn hóa dữ liệu
        foreach (var i in req.Items)
        {
            if (i.Quantity <= 0) throw new InvalidOperationException("Quantity phải > 0");
            if (i.ImportPrice.HasValue && i.ImportPrice.Value < 0) throw new InvalidOperationException("ImportPrice không âm");
        }

        // Tạo InvoiceNumber nếu thiếu
        var now = DateTime.UtcNow;
        var orderDate = req.OrderDate ?? DateOnly.FromDateTime(now);
        var invoice = string.IsNullOrWhiteSpace(req.InvoiceNumber)
            ? $"IMP-{orderDate:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}"
            : req.InvoiceNumber!.Trim();

        var order = new ImportOrder
        {
            ProviderId = req.ProviderId,
            InvoiceNumber = invoice,
            OrderDate = orderDate,
            CreatedDate = DateOnly.FromDateTime(now),
            Status = "Pending",          // 👉 dùng đúng string đang có
            CreatedBy = createdBy
        };

        _ctx.ImportOrders.Add(order);
        await _ctx.SaveChangesAsync(ct); // để có ImportOrderId

        var details = req.Items.Select(i => new ImportDetail
        {
            ImportOrderId = order.ImportOrderId,
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            ImportPrice = i.ImportPrice
        });

        _ctx.ImportDetails.AddRange(details);
        await _ctx.SaveChangesAsync(ct);

        return new CreateImportOrderResponse(order.ImportOrderId, order.InvoiceNumber);
    }

    /// <summary>
    /// Review phiếu nhập:
    /// - approve = true  => Pending -> Completed, cộng QuantityAvailable cho Inventory
    /// - approve = false => Pending -> Canceled, KHÔNG cộng tồn
    /// </summary>
    public async Task<bool> ReviewAsync(
         int importOrderId,
         int reviewerId,
         bool approve,
         string? note,
         CancellationToken ct = default)
    {
        var order = await _ctx.ImportOrders
            .Include(o => o.ImportDetails)
            .FirstOrDefaultAsync(o => o.ImportOrderId == importOrderId, ct);

        if (order == null)
            throw new InvalidOperationException("Import order không tồn tại.");

        // chỉ cho review khi đang Pending
        if (!string.Equals(order.Status, "Pending", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Chỉ được review phiếu ở trạng thái Pending.");

        // ❌ HỦY PHIẾU: Pending -> Canceled (không động tới tồn, không transaction log)
        if (!approve)
        {
            order.Status = "Canceled";

            // ghi ActionLog
            _ctx.ActionLogs.Add(new ActionLog
            {
                UserId = reviewerId,
                ActionType = "IMPORT_CANCELED",
                EntityType = "ImportOrder",
                Timestamp = DateTime.UtcNow,
                Description = $"Cancel import order #{order.ImportOrderId}. Note: {note}"
            });

            await _ctx.SaveChangesAsync(ct);
            return true;
        }

        // ✅ DUYỆT PHIẾU: Pending -> Completed
        // 1. Cộng tồn kho + ghi TransactionLog từng dòng
        foreach (var detail in order.ImportDetails)
        {
            // Inventory đang key theo ProductId (bảng Inventory bạn gửi chỉ có ProductId, QuantityAvailable, AllocatedQuantity, LocationID)
            var inventory = await _ctx.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == detail.ProductId, ct);

            var changeQty = detail.Quantity;                    // số lượng nhập thêm
            var oldQty = inventory?.QuantityAvailable ?? 0;
            var newQty = oldQty + changeQty;

            if (inventory == null)
            {
                inventory = new Inventory
                {
                    ProductId = detail.ProductId,
                    QuantityAvailable = newQty,
                    // nếu LocationId là NOT NULL thì set mặc định hoặc sửa logic sau
                    LocationId = 1,          // TODO: chỉnh lại nếu bạn muốn map theo location thật
                    AllocatedQuantity = 0
                };
                await _ctx.Inventories.AddAsync(inventory, ct);
            }
            else
            {
                inventory.QuantityAvailable = newQty;
            }

            // ghi TransactionLog cho dòng này
            _ctx.TransactionLogs.Add(new TransactionLog
            {
                OrderId = order.ImportOrderId,
                ProductId = detail.ProductId,
                Quantity = newQty,             // tồn mới sau khi cộng
                QuantityChanged = changeQty,   // số lượng tăng
                CreatedBy = reviewerId,
                Type = "IMPORT",
                TransactionDate = DateTime.UtcNow,
                Notes = note
            });
        }

        // 2. Cập nhật trạng thái phiếu nhập
        order.Status = "Completed";

        // 3. Ghi ActionLog cho hành động Completed
        _ctx.ActionLogs.Add(new ActionLog
        {
            UserId = reviewerId,
            ActionType = "IMPORT_COMPLETED",
            EntityType = "ImportOrder",
            Timestamp = DateTime.UtcNow,
            Description = $"Complete import order #{order.ImportOrderId}. Note: {note}"
        });

        await _ctx.SaveChangesAsync(ct);
        return true;
    }
}
