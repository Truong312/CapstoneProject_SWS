using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class TransactionLog
{
    public int TransactionId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public int? CreatedBy { get; set; }

    public string? Type { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string? Notes { get; set; }

    public int? QuantityChanged { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Product? Product { get; set; }
}
