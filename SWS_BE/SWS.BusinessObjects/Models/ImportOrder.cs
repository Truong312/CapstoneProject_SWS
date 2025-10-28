using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class ImportOrder
{
    public int ImportOrderId { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public DateOnly OrderDate { get; set; }

    public int ProviderId { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public DateOnly? ArrivalDate { get; set; }

    public string? Status { get; set; }

    public int? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();

    public virtual BusinessPartner Provider { get; set; } = null!;
}
