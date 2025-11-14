using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class CycleCount
{
    public int CycleCountId { get; set; }

    public string CycleName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<CycleCountDetail> CycleCountDetails { get; set; } = new List<CycleCountDetail>();
}
