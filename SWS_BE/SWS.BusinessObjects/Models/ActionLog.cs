using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.Models;

public partial class ActionLog
{
    public int ActionId { get; set; }

    public int UserId { get; set; }

    public string? ActionType { get; set; }

    public string? EntityType { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? Description { get; set; }

    public virtual User User { get; set; } = null!;
}
