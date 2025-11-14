using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int Role { get; set; }

    public virtual ICollection<ActionLog> ActionLogs { get; set; } = new List<ActionLog>();

    public virtual ICollection<CycleCountDetail> CycleCountDetails { get; set; } = new List<CycleCountDetail>();

    public virtual ICollection<ExportOrder> ExportOrders { get; set; } = new List<ExportOrder>();

    public virtual ICollection<ImportOrder> ImportOrders { get; set; } = new List<ImportOrder>();

    public virtual ICollection<ReturnOrder> ReturnOrderCheckedByNavigations { get; set; } = new List<ReturnOrder>();

    public virtual ICollection<ReturnOrder> ReturnOrderReviewedByNavigations { get; set; } = new List<ReturnOrder>();

    public virtual ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();
}
