using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels;
using SWS.Services.ApiModels.ExportOrderModel;

namespace SWS.Services.Services.ExportOrderServices
{
    public class ExportOrderService : IExportOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ExportOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultModel<IEnumerable<ExportOrderResponse>>> GetAllExportOrdersAsync()
        {
            try
            {
                var exportOrders = await _unitOfWork.ExportOrders.GetAllAsync();
                var result = exportOrders.Select(e => new ExportOrderResponse
                {
                    ExportOrderId = e.ExportOrderId,
                    InvoiceNumber = e.InvoiceNumber,
                    OrderDate = e.OrderDate,
                    CustomerId = e.CustomerId,
                    Currency = e.Currency,
                    CreatedDate = e.CreatedDate,
                    ShippedDate = e.ShippedDate,
                    ShippedAddress = e.ShippedAddress,
                    TaxRate = e.TaxRate,
                    TaxAmount = e.TaxAmount,
                    TotalPayment = e.TotalPayment,
                    Description = e.Description,
                    Status = e.Status,
                    CreatedBy = e.CreatedBy
                });
                return new ResultModel<IEnumerable<ExportOrderResponse>>
                {
                    IsSuccess = true,
                    Message = "Danh sách hóa đơn xuất hàng",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<IEnumerable<ExportOrderResponse>>
                {
                    IsSuccess = false,
                    Message = "Lỗi khi lấy danh sách hóa đơn xuất hàng",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel<IEnumerable<ExportDetailResponse>>> GetExportOrderDetails(int exportOrderId)
        {
            try
            {
                var exportDetails = await _unitOfWork.ExportOrders.GetExportDetails(exportOrderId);
                var result = exportDetails.Select(e => new ExportDetailResponse
                {
                    ExportDetailId=e.ExportDetailId,
                    ExportOrderId=e.ExportOrderId,
                    ProductId=e.ProductId,
                    Quantity=e.Quantity,
                    TotalPrice=e.TotalPrice
                });
                return new ResultModel<IEnumerable<ExportDetailResponse>>
                {
                    IsSuccess = true,
                    Message = "Thông tin chi tiết hóa đơn xuất hàng",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<IEnumerable<ExportDetailResponse>>
                {
                    IsSuccess = false,
                    Message = "Lỗi khi lấy thông tin chi tiết hóa đơn xuất hàng",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
