using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Enums;
using SWS.Services.ApiModels.Commons;

namespace SWS.Services.Services.LogServices
{
    public interface IActionLogService
    {
        public int GetCurrentUserId();
        public Task<ResultModel> CreateActionLogAsync(ActionType actionType, string entityType, string description);
    }
}
