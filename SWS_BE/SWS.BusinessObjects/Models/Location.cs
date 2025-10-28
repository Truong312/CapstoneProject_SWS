using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public string? ShelfId { get; set; }

    public int? ColumnNumber { get; set; }

    public int? RowNumber { get; set; }

    public string? Type { get; set; }

    public bool? IsFull { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<ReturnOrderDetail> ReturnOrderDetails { get; set; } = new List<ReturnOrderDetail>();
}
