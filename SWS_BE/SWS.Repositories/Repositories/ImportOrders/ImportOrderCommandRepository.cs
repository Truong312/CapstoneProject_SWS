using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.DTOs;
using SWS.BusinessObjects.Models;

namespace SWS.Repositories.Repositories.ImportOrders;

public class ImportOrderCommandRepository : IImportOrderCommandRepository
{
    private readonly SmartWarehouseDbContext _ctx;
    public ImportOrderCommandRepository(SmartWarehouseDbContext ctx) => _ctx = ctx;

    public async Task<CreateImportOrderResponse> CreateAsync(int createdBy, CreateImportOrderRequest req, CancellationToken ct = default)
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
            Status = "Pending",
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
}
