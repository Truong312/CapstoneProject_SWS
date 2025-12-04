using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.LogModel;

namespace SWS.Services.Services.LogServices
{
    public class ActionLogService : IActionLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IHttpContextAccessor _httpContextAccessor;
        public ActionLogService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        public int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new Exception("User is not authenticated.");
            }
            return int.Parse(userIdClaim);
        }
        public async Task<ResultModel> CreateActionLogAsync(ActionType actionType, string entityType, string description)
        {
            try
            {
                var actionLogApi = new ActionLogResponse
                {
                    UserId = GetCurrentUserId(),
                    ActionType=actionType,
                    EntityType=entityType,
                    Timestamp=DateTime.Now,
                    Description= description
                };
                var actionLog = new ActionLog
                {
                    UserId=actionLogApi.UserId,
                    ActionType=actionLogApi.ActionType.ToString(),
                    EntityType=actionLogApi.EntityType,
                    Timestamp=actionLogApi.Timestamp,
                    Description=actionLogApi.Description
                };
                await _unitOfWork.ActionLogs.AddAsync(actionLog);
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess=true,
                    Message=$"Tạo action log",
                    StatusCode=StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tạo action log",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
