using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels;
using SWS.Services.ApiModels.ExportDetailModel;
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
        private StatusEnums? ParseStatus(string? status)
        {
            return Enum.TryParse<StatusEnums>(status, true, out var parsedStatus)
                ? parsedStatus
                : (StatusEnums?)null;
        }

        private async Task<decimal?> GetUnitPriceAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            return product?.UnitPrice;
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
                    Status = ParseStatus(e.Status),
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

        public async Task<ResultModel<IEnumerable<ExportOrderResponse>>> GetExportOrdersByStatusAsync(StatusEnums status)
        {
            try
            {
                var exportOrders = await _unitOfWork.ExportOrders.GetAllAsync();
                var filtered = exportOrders
                    .Where(e => string.Equals(e.Status, status.ToString(), StringComparison.OrdinalIgnoreCase))
                    .Select(e => new ExportOrderResponse
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
                        Status = ParseStatus(e.Status),
                        CreatedBy = e.CreatedBy
                    });

                return new ResultModel<IEnumerable<ExportOrderResponse>>
                {
                    IsSuccess = true,
                    Message = $"Danh sách hóa đơn xuất hàng có trạng thái {status}",
                    Data = filtered,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<IEnumerable<ExportOrderResponse>>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lọc danh sách hóa đơn xuất hàng: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel<IEnumerable<ExportDetailResponse>>> GetExportDetails(int exportOrderId)
        {
            try
            {
                var exportDetails = await _unitOfWork.ExportOrders.GetExportDetails(exportOrderId);
                var result = exportDetails.Select(e => new ExportDetailResponse
                {
                    ExportDetailId = e.ExportDetailId,
                    ExportOrderId = e.ExportOrderId,
                    ProductId = e.ProductId,
                    Quantity = e.Quantity,
                    TotalPrice = e.TotalPrice
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
        public async Task<ResultModel<ExportOrderResponse>> GetExportOrderByIdAsync(int exportOrderId)
        {
            try
            {
                var exportOrder = await _unitOfWork.ExportOrders.GetByIdAsync(exportOrderId);
                if (exportOrder == null)
                {
                    return new ResultModel<ExportOrderResponse>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy hóa đơn xuất hàng",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                var result = new ExportOrderResponse
                {
                    ExportOrderId = exportOrder.ExportOrderId,
                    InvoiceNumber = exportOrder.InvoiceNumber,
                    OrderDate = exportOrder.OrderDate,
                    CustomerId = exportOrder.CustomerId,
                    Currency = exportOrder.Currency,
                    CreatedDate = exportOrder.CreatedDate,
                    ShippedDate = exportOrder.ShippedDate,
                    ShippedAddress = exportOrder.ShippedAddress,
                    TaxRate = exportOrder.TaxRate,
                    TaxAmount = exportOrder.TaxAmount,
                    TotalPayment = exportOrder.TotalPayment,
                    Description = exportOrder.Description,
                    Status = ParseStatus(exportOrder.Status),
                    CreatedBy = exportOrder.CreatedBy
                };
                return new ResultModel<ExportOrderResponse>
                {
                    IsSuccess = true,
                    Message = "Hóa đơn xuất hàng: ",
                    Data=result,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<ExportOrderResponse>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tìm thấy hóa đơn xuất hàng{e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel<ExportDetailResponse>> GetExportDetailByIdAsync(int exportDetailId)
        {
            try
            {
                var exportDetail = await _unitOfWork.ExportDetails.GetByIdAsync(exportDetailId);
                if (exportDetail == null)
                {
                    return new ResultModel<ExportDetailResponse>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy thông tin chi tiết hóa đơn xuất hàng",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                var result = new ExportDetailResponse
                {
                    ExportDetailId = exportDetail.ExportDetailId,
                    ExportOrderId = exportDetail.ExportOrderId,
                    ProductId = exportDetail.ProductId,
                    Quantity = exportDetail.Quantity,
                    TotalPrice = exportDetail.TotalPrice
                };
                return new ResultModel<ExportDetailResponse>
                {
                    IsSuccess = true,
                    Message = "Thông tin chi tiết hóa đơn xuất hàng",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<ExportDetailResponse>
                {
                    IsSuccess = false,
                    Message = "Lỗi khi lấy thông tin chi tiết hóa đơn xuất hàng",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> AddExportOrderAsync(CreateExportOrder createExportOrder)
        {
            try
            {
                var exportOrder = new ExportOrder
                {
                    InvoiceNumber = createExportOrder.InvoiceNumber,
                    OrderDate = createExportOrder.OrderDate,
                    CustomerId = createExportOrder.CustomerId,
                    Currency = createExportOrder.Currency,
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                    ShippedDate = createExportOrder.ShippedDate,
                    ShippedAddress = createExportOrder.ShippedAddress,
                    TaxRate = createExportOrder.TaxRate,
                    TaxAmount = createExportOrder.TaxAmount,
                    TotalPayment = createExportOrder.TotalPayment,
                    Description = createExportOrder.Description,
                    Status = StatusEnums.Pending.ToString(),
                    CreatedBy = createExportOrder.CreatedBy
                };
                await _unitOfWork.ExportOrders.AddAsync(exportOrder);
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Thêm hoá đơn xuất hàng thành công",
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi thêm hoá đơn xuất hàng: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> AddExportDetailAsync(int exportOrderId, CreateExportDetail createExportDetail)
        {
            try
            {
                var exportOrder = await _unitOfWork.ExportOrders.GetByIdAsync(exportOrderId);
                if (exportOrder == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = " thông tin chi tiết hoá đơn xuất hàng này không thuộc bất kỳ hóa đơn xuất hàng nào",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var exportDetail = new ExportDetail
                {
                    ExportOrderId = exportOrderId,
                    ProductId = createExportDetail.ProductId,
                    Quantity = createExportDetail.Quantity,
                    TotalPrice = (await GetUnitPriceAsync(createExportDetail.ProductId)) * createExportDetail.Quantity

                };
                await _unitOfWork.ExportDetails.AddAsync(exportDetail);
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Thêm thông tin chi tiết hoá đơn xuất hàng thành công",
                    StatusCode = StatusCodes.Status201Created
                }
                ;
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi thêm thông tin chi tiết hoá đơn xuất hàng: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> UpdateExportOrderAsync(int exportOrderId, UpdateExportOrder updateExportOrder)
        {
            try
            {
                var exportOrder = await _unitOfWork.ExportOrders.GetByIdAsync(exportOrderId);
                if (exportOrder == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Không thể cập nhật hoá đơn xuất hàng",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                if (updateExportOrder.OrderDate.HasValue) exportOrder.OrderDate = updateExportOrder.OrderDate.Value;
                if (updateExportOrder.CustomerId.HasValue) exportOrder.CustomerId = updateExportOrder.CustomerId.Value;
                if (updateExportOrder.CreatedDate.HasValue) exportOrder.CreatedDate = updateExportOrder.CreatedDate.Value;
                if (updateExportOrder.ShippedDate.HasValue) exportOrder.ShippedDate = updateExportOrder.ShippedDate.Value;
                if (!string.IsNullOrEmpty(updateExportOrder.ShippedAddress)) exportOrder.ShippedAddress = updateExportOrder.ShippedAddress;
                if (updateExportOrder.TaxRate.HasValue) exportOrder.TaxRate = updateExportOrder.TaxRate.Value;
                if (updateExportOrder.TaxAmount.HasValue) exportOrder.TaxAmount = updateExportOrder.TaxAmount.Value;
                if (updateExportOrder.TotalPayment.HasValue) exportOrder.TotalPayment = updateExportOrder.TotalPayment.Value;
                if (!string.IsNullOrEmpty(updateExportOrder.Description)) exportOrder.Description = updateExportOrder.Description;
                if (!string.IsNullOrEmpty(updateExportOrder.Status))
                {
                    if (Enum.TryParse<StatusEnums>(updateExportOrder.Status, true, out var validStatus))
                    {
                        exportOrder.Status = validStatus.ToString();
                    }
                    else
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            Message = $"Trạng thái '{updateExportOrder.Status}' không hợp lệ. Vui lòng dùng: {string.Join(", ", Enum.GetNames(typeof(StatusEnums)))}",
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }
                }
                if (updateExportOrder.CreatedBy.HasValue) exportOrder.CreatedBy = updateExportOrder.CreatedBy.Value;
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Cập nhật hoá đơn xuất hàng thành công",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi cập nhật hoá đơn xuất hàng: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> UpdateExportDetailAsync(int exportDetailId, UpdateExportDetail updateExportDetail)
        {
            try
            {
                var exportDetail = await _unitOfWork.ExportDetails.GetByIdAsync(exportDetailId);
                if (exportDetail == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Không thể cập nhật thông tin chi tiết hoá đơn xuất hàng",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                if (updateExportDetail.ProductId.HasValue)
                    exportDetail.ProductId = updateExportDetail.ProductId.Value;

                if (updateExportDetail.Quantity.HasValue)
                {
                    exportDetail.Quantity = updateExportDetail.Quantity.Value;
                    var unitPrice = await GetUnitPriceAsync(exportDetail.ProductId);
                    if (unitPrice == null)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            Message = "Không tìm thấy giá sản phẩm",
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }
                    exportDetail.TotalPrice = unitPrice * exportDetail.Quantity;
                }

                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Cập nhật thông tin chi tiết hoá đơn xuất hàng thành công",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi cập nhật thông tin chi tiết hoá đơn xuất hàng: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> DeleteExportOrderAsync(int exportOrderId)
        {
            try
            {
                var exportOrder = await _unitOfWork.ExportOrders.GetByIdAsync(exportOrderId);
                if (exportOrder == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy hoá đơn xuất hàng để xóa",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                var exportDetails = await _unitOfWork.ExportDetails.GetAllByExportOrderId(exportOrderId);
                foreach (var exportDetail in exportDetails)
                {
                    await _unitOfWork.ExportDetails.DeleteAsync(exportDetail);
                }
                await _unitOfWork.ExportOrders.DeleteAsync(exportOrder);
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Xóa hoá đơn xuất hàng thành công",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi xóa hoá đơn xuất hàng: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> DeleteExportDetailAsync(int exportDetailId)
        {
            try
            {
                var exportDetail = await _unitOfWork.ExportDetails.GetByIdAsync(exportDetailId);
                if (exportDetail == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy thông tin chi tiết hoá đơn xuất hàng để xóa",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                await _unitOfWork.ExportDetails.DeleteAsync(exportDetail);
                await _unitOfWork.SaveChangesAsync();

                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Xóa thông tin chi tiết hoá đơn xuất hàng thành công",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi xóa thông tin chi tiết hoá đơn xuất hàng: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
