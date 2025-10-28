using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class ReturnAction
{
    public int ActionId { get; set; }

    public string? Action { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<ReturnOrderDetail> ReturnOrderDetails { get; set; } = new List<ReturnOrderDetail>();
}
