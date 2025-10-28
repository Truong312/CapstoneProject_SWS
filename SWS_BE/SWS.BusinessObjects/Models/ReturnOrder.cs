using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class ReturnOrder
{
    public int ReturnOrderId { get; set; }

    public int? ExportOrderId { get; set; }

    public int? CheckedBy { get; set; }

    public int? ReviewedBy { get; set; }

    public DateTime? CheckInTime { get; set; }

    public string? Status { get; set; }

    public string? Note { get; set; }

    public virtual User? CheckedByNavigation { get; set; }

    public virtual ExportOrder? ExportOrder { get; set; }

    public virtual ICollection<ReturnOrderDetail> ReturnOrderDetails { get; set; } = new List<ReturnOrderDetail>();

    public virtual User? ReviewedByNavigation { get; set; }
}
