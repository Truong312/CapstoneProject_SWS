using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class BusinessPartner
{
    public int PartnerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<ExportOrder> ExportOrders { get; set; } = new List<ExportOrder>();

    public virtual ICollection<ImportOrder> ImportOrders { get; set; } = new List<ImportOrder>();
}
