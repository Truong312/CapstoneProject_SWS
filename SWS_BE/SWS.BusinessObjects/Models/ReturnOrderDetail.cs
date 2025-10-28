using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class ReturnOrderDetail
{
    public int ReturnDetailId { get; set; }

    public int ReturnOrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string? Note { get; set; }

    public int? ReasonId { get; set; }

    public int? ActionId { get; set; }

    public int? LocationId { get; set; }

    public virtual ReturnAction? Action { get; set; }

    public virtual Location? Location { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ReturnReason? Reason { get; set; }

    public virtual ReturnOrder ReturnOrder { get; set; } = null!;
}
