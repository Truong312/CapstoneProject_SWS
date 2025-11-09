using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string SerialNumber { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateOnly ExpiredDate { get; set; }

    public string? Unit { get; set; }

    public decimal? UnitPrice { get; set; }

    public DateOnly ReceivedDate { get; set; }

    public decimal? PurchasedPrice { get; set; }

    public int? ReorderPoint { get; set; }

    public string? Image { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<CycleCountDetail> CycleCountDetails { get; set; } = new List<CycleCountDetail>();

    public virtual ICollection<ExportDetail> ExportDetails { get; set; } = new List<ExportDetail>();

    public virtual ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<ReturnOrderDetail> ReturnOrderDetails { get; set; } = new List<ReturnOrderDetail>();

    public virtual ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();
}
