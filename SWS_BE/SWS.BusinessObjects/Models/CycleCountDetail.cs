using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class CycleCountDetail
{
    public int DetailId { get; set; }

    public int CycleCountId { get; set; }

    public int ProductId { get; set; }

    public int SystemQuantity { get; set; }

    public int CountedQuantity { get; set; }

    public int? Difference { get; set; }

    public int CheckedBy { get; set; }

    public DateTime? CheckedDate { get; set; }

    public string? Notes { get; set; }

    public virtual User CheckedByNavigation { get; set; } = null!;

    public virtual CycleCount CycleCount { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
