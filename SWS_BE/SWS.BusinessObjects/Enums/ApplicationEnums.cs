using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.BusinessObjects.Enums
{
    public enum ApplicationEnums
    {
    }

    public enum Status
    {
        Active,
        Inactive,
        Deleted,
        Completed,
        Pending
    }
    public enum EmployeeType
    {
        Admin,
        Manager,
        Staff
    }
    public enum StatusEnums
    {
        Pending,
        Shipped,
        Completed,
        Canceled
    }
    public enum InventoryStatus
    {
        Available = 1,
        Allocated = 2,
        Damaged = 3,
        InTransit = 4
    }
    public enum ActionType//for actionLog
    {
        Create =1,
        Update=2,
        Delete=3,
        Login=4,
        Logout=5
    }
    public enum TransactionType//for transactionLog
    {
        Export=1,
        Import=2
    }
}
