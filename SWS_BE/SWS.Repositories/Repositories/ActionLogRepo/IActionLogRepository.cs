using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.ActionLogRepo
{
    public interface IActionLogRepository:IGenericRepository<ActionLog>
    {
        Task<IEnumerable<ActionLog>> GetWithCondition(DateOnly? startDate, DateOnly? endDate, ActionType? actionType);
    }
}
