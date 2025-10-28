using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class ExportOrder
{
    public int ExportOrderId { get; set; }

    public string? InvoiceNumber { get; set; }

    public DateOnly OrderDate { get; set; }

    public int CustomerId { get; set; }

    public string? Currency { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public DateOnly? ShippedDate { get; set; }

    public string? ShippedAddress { get; set; }

    public decimal? TaxRate { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal? TotalPayment { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public int? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual BusinessPartner Customer { get; set; } = null!;

    public virtual ICollection<ExportDetail> ExportDetails { get; set; } = new List<ExportDetail>();

    public virtual ICollection<ReturnOrder> ReturnOrders { get; set; } = new List<ReturnOrder>();
}
