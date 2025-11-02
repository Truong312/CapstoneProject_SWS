using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.DTOs;
using SWS.BusinessObjects.Models;

namespace SWS.Repositories.Repositories.ImportOrders;

public class ImportOrderQueryRepository : IImportOrderQueryRepository
{
    private readonly SmartWarehouseDbContext _ctx;
    public ImportOrderQueryRepository(SmartWarehouseDbContext ctx) => _ctx = ctx;

    public async Task<ImportOrderListResult> GetListAsync(ImportOrderListQuery q, CancellationToken ct = default)
    {
        var baseQuery = _ctx.ImportOrders
            .AsNoTracking()
            .Include(o => o.Provider)
            .Include(o => o.CreatedByNavigation)
            .Include(o => o.ImportDetails)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q.Q))
        {
            var term = q.Q.Trim();
            baseQuery = baseQuery.Where(o =>
                   o.InvoiceNumber.Contains(term)
                || (o.Provider.Name != null && o.Provider.Name.Contains(term)));
        }

        if (q.ProviderId.HasValue) baseQuery = baseQuery.Where(o => o.ProviderId == q.ProviderId.Value);
        if (!string.IsNullOrWhiteSpace(q.Status)) baseQuery = baseQuery.Where(o => o.Status == q.Status);

        if (q.From.HasValue) baseQuery = baseQuery.Where(o => o.OrderDate >= q.From.Value);
        if (q.To.HasValue) baseQuery = baseQuery.Where(o => o.OrderDate <= q.To.Value);

        var total = await baseQuery.CountAsync(ct);

        var items = await baseQuery
            .OrderByDescending(o => o.ImportOrderId)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(o => new ImportOrderListItemDto(
                o.ImportOrderId,
                o.InvoiceNumber,
                o.OrderDate,
                o.Provider.Name,
                o.Status,
                o.ImportDetails.Sum(d => d.Quantity),
                o.CreatedByNavigation != null ? o.CreatedByNavigation.FullName : null
            ))
            .ToListAsync(ct);

        return new ImportOrderListResult(total, q.Page, q.PageSize, items);
    }

    public async Task<ImportOrderDetailDto?> GetDetailAsync(int id, CancellationToken ct = default)
    {
        var order = await _ctx.ImportOrders
            .AsNoTracking()
            .Include(o => o.Provider)
            .Include(o => o.CreatedByNavigation)
            .Include(o => o.ImportDetails).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.ImportOrderId == id, ct);

        if (order == null) return null;

        var items = order.ImportDetails.Select(d => new ImportOrderDetailItemDto(
            d.ImportDetailId, d.ProductId, d.Product.Name, d.Quantity, d.ImportPrice
        ));

        return new ImportOrderDetailDto(
            order.ImportOrderId,
            order.InvoiceNumber,
            order.OrderDate,
            order.ProviderId,
            order.Provider.Name,
            order.Status,
            order.CreatedDate,
            order.CreatedBy,
            order.CreatedByNavigation?.FullName,
            items
        );
    }
}
