using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class ImportDetail
{
    public int ImportDetailId { get; set; }

    public int ImportOrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal? ImportPrice { get; set; }

    public virtual ImportOrder ImportOrder { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
