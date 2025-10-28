using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class ReturnReason
{
    public int ReasonId { get; set; }

    public string? ReasonCode { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<ReturnOrderDetail> ReturnOrderDetails { get; set; } = new List<ReturnOrderDetail>();
}
