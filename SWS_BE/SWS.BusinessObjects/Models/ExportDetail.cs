using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class ExportDetail
{
    public int ExportDetailId { get; set; }

    public int ExportOrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual ExportOrder ExportOrder { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
