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
using SWS.Services.Services.LogServices;

namespace SWS.Services.Services.CycleCountServices
{
    public class CycleCountService : ICycleCountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionLogService _actionLogService;

        public CycleCountService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,IActionLogService actionLogService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _actionLogService = actionLogService;
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
        public async Task<ResultModel> StartCycleCountAsync()
        {
            try
            {
                var userId= GetCurrentUserId();
                var now = DateOnly.FromDateTime(DateTime.Now);
                var cycleName = $"Q{((now.Month) - 1) / 3 + 1}_{now.Year}";
                var cycleCheck = await _unitOfWork.CycleCounts.GetByName(cycleName);
                if (cycleCheck != null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = $"Đã tồn tại cycle count trong quý này:{cycleName}",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var start = new DateOnly(now.Year, ((now.Month - 1) / 3) * 3 + 1, 1);//example now = 27/07/2025 -> start = 01/06/2025
                var end = start.AddMonths(3).AddDays(-1);//example start = 01/06/2025 -> end = 31/08/2025

                var cycle = new CycleCount
                {
                    CycleName = cycleName,
                    StartDate = start,
                    EndDate = end,
                    CreatedBy = userId,
                    Status = StatusEnums.Pending.ToString()
                };
                await _unitOfWork.CycleCounts.AddAsync(cycle);
                await _unitOfWork.SaveChangesAsync();

                var products = await _unitOfWork.Products.GetAllAsync();
                foreach (var p in products)
                {
                    var detail = new CycleCountDetail
                    {
                        CycleCountId = cycle.CycleCountId,
                        ProductId = p.ProductId,
                        SystemQuantity = await _unitOfWork.Products.GetProductQuantity(p.ProductId),
                        CountedQuantity = await _unitOfWork.Products.GetProductQuantity(p.ProductId),
                        CheckedBy = userId
                    };
                    await _unitOfWork.CycleCountDetails.AddAsync(detail);
                }
                await _actionLogService.CreateActionLogAsync(ActionType.Create,"CycleCount",$"Đã tạo cycle count mới với tên {cycle.CycleName}");
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = $"Đã tạo cycle count mới với tên {cycle.CycleName}",
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi thêm  tạo cycle count: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> UpdateCountedQuantityAsync(int detailId, int countedQuantity)
        {
            try
            {
                var userId = GetCurrentUserId();
                var detail = await _unitOfWork.CycleCountDetails.GetByIdAsync(detailId);
                if (detail == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy cycle count detail",
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
                detail.CountedQuantity = countedQuantity;
                detail.CheckedDate = DateTime.Now;
                await  _actionLogService.CreateActionLogAsync(ActionType.Update, "CycleCountDetail", $"Update Cycle count detail id{detail.DetailId}");
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Đã cập nhật cycle count quantity",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi xảy ra khi cập nhật cycle count quantity: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> FinalizeCycleCountAsync(int cycleCountId)
        {
            try
            {
                //get CycleCount
                var cycle = await _unitOfWork.CycleCounts.GetByIdAsync(cycleCountId);
                if (cycle == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = $"Không tìm được cycle count với id: {cycleCountId}",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                var details = await _unitOfWork.CycleCountDetails.GetAllByCycleCountId(cycle.CycleCountId);
                //Check if there is discrepancy in quantity
                foreach (var detail in details)
                {
                    var inventory = await _unitOfWork.Inventories.GetByProductId(detail.ProductId);
                    if (inventory != null && inventory.QuantityAvailable != detail.CountedQuantity)
                    {
                        var difference = detail.CountedQuantity - inventory.QuantityAvailable;
                        inventory.QuantityAvailable = detail.CountedQuantity;

                        var product = await _unitOfWork.Products.GetByIdAsync(detail.ProductId);
                        if (product == null)
                        {
                            return new ResultModel
                            {
                                IsSuccess = false,
                                Message = $"Lỗi xảy ra khi tìm product với id: {detail.ProductId}",
                                StatusCode = StatusCodes.Status500InternalServerError
                            };
                        }
                        await _actionLogService.CreateActionLogAsync(ActionType.Update, "CycleCountDetail", $"Adjust quantity of {product.SerialNumber}");
                    }
                }
                //CycleCount Task has been completed
                cycle.Status = StatusEnums.Completed.ToString();
                await _actionLogService.CreateActionLogAsync(ActionType.Update, "CycleCountDetail", $"Finalize Cycle count{cycle.CycleCountId}");
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Đã cập nhật cycle count",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi xảy ra khi chốt cycle count: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> FinalizeCycleCountAsync(string cycleCountName)
        {
            try
            {
                //get CycleCount
                var cycle = await _unitOfWork.CycleCounts.GetByName(cycleCountName);
                if (cycle == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = $"Không tìm được cycle count với id: {cycleCountName}",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                var details = await _unitOfWork.CycleCountDetails.GetAllByCycleCountId(cycle.CycleCountId);
                //Check if there is discrepancy in quantity
                foreach (var detail in details)
                {
                    var inventory = await _unitOfWork.Inventories.GetByProductId(detail.ProductId);
                    if (inventory != null && inventory.QuantityAvailable != detail.CountedQuantity)
                    {
                        var difference = detail.CountedQuantity - inventory.QuantityAvailable;
                        inventory.QuantityAvailable = detail.CountedQuantity;

                        var product = await _unitOfWork.Products.GetByIdAsync(detail.ProductId);
                        if (product == null)
                        {
                            return new ResultModel
                            {
                                IsSuccess = false,
                                Message = $"Lỗi xảy ra khi tìm product với id: {detail.ProductId}",
                                StatusCode = StatusCodes.Status500InternalServerError
                            };
                        }
                        //Action Logs
                        await _actionLogService.CreateActionLogAsync(ActionType.Update, "CycleCountDetail", $"Adjust quantity of {product.SerialNumber}");
                    }
                }
                //CycleCount Task has been completed
                cycle.Status = StatusEnums.Completed.ToString();
                await _actionLogService.CreateActionLogAsync(ActionType.Update, "CycleCountDetail", $"Finalize Cycle count{cycle.CycleName}");
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Đã cập nhật cycle count",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi xảy ra khi chốt cycle count: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
