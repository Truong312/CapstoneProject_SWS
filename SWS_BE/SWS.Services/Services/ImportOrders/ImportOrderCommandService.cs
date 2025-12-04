using Microsoft.AspNetCore.Http;
using SWS.BusinessObjects.DTOs;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.ImportOrders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SWS.Services.ImportOrders
{
    public class ImportOrderCommandService : IImportOrderCommandService
    {
        private readonly IUnitOfWork _uow;

        public ImportOrderCommandService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<CreateImportOrderResponse> CreateAsync(
            int createdBy,
            CreateImportOrderRequest req,
            CancellationToken ct = default)
            => _uow.ImportOrdersCommand.CreateAsync(createdBy, req, ct);

        public async Task<ResultModel<bool>> ReviewAsync(
            int importOrderId,
            int reviewerId,
            ReviewImportOrderRequest req,
            CancellationToken ct = default)
        {
            try
            {
                var ok = await _uow.ImportOrdersCommand
                    .ReviewAsync(importOrderId, reviewerId, req.Approve, req.Note, ct);

                var message = req.Approve
                    ? "Duyệt phiếu nhập (COMPLETED) và cập nhật tồn kho thành công."
                    : "Đã hủy phiếu nhập (CANCELED).";

                return new ResultModel<bool>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = message,
                    Data = ok
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<bool>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    Data = false
                };
            }
        }
    }
}
