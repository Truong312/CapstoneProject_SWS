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
    public class TransactionLogService : ITransactionLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TransactionLogService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new Exception("User is not authenticated.");
            }
            return int.Parse(userIdClaim);
        }
        public async Task<ResultModel> CreateTransactionLogAsync(int orderId, int productId, int quantity,TransactionType type, string? note, int quantityChange)
        {
            try
            {
                var transactionLogApi = new TransactionLogResponse
                {
                    OrderId = orderId,
                    ProductId = productId,
                    Quantity = quantity,
                    CreatedBy = GetCurrentUserId(),
                    Type = type.ToString(),
                    TransactionDate=DateTime.Now,
                    Notes=note,
                    QuantityChanged=quantityChange
                };
                var transactionLog = new TransactionLog
                {
                    OrderId = transactionLogApi.OrderId,
                    ProductId = transactionLogApi.ProductId,
                    Quantity = transactionLogApi.Quantity,
                    CreatedBy = transactionLogApi.CreatedBy,
                    Type = transactionLogApi.Type,
                    TransactionDate = transactionLogApi.TransactionDate,
                    Notes = transactionLogApi.Notes,
                    QuantityChanged = transactionLogApi.QuantityChanged
                };
                await _unitOfWork.TransactionLogs.AddAsync(transactionLog);
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = $"Tạo transaction log",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tạo transaction log: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
