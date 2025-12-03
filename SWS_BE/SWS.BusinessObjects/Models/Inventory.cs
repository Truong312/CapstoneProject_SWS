using SWS.BusinessObjects.Enums;
using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int ProductId { get; set; }

    public int QuantityAvailable { get; set; }

    public int AllocatedQuantity { get; set; }

    public int LocationId { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

   
}
