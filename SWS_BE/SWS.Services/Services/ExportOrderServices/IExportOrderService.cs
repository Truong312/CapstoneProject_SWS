using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.ExportDetailModel;
using SWS.Services.ApiModels.ExportOrderModel;

namespace SWS.Services.Services.ExportOrderServices
{
    public interface IExportOrderService
    {
        Task<ResultModel<IEnumerable<ExportOrderResponse>>> GetAllExportOrdersAsync();
        Task<ResultModel<IEnumerable<ExportOrderResponse>>> GetExportOrdersByStatusAsync(StatusEnums status);
        Task<ResultModel<IEnumerable<ExportDetailResponse>>> GetExportDetails(int exportOrderId);
        Task<ResultModel<ExportOrderResponse>> GetExportOrderByIdAsync(int exportOrderId);
        Task<ResultModel<ExportDetailResponse>> GetExportDetailByIdAsync(int exportDetailId);
        Task<ResultModel> AddExportOrderAsync(CreateExportOrder createExportOrder);
        Task<ResultModel> AddExportDetailAsync(int exportOrderId, CreateExportDetail createExportDetail);
        Task<ResultModel> UpdateExportOrderAsync(int exportOrderId, UpdateExportOrder updateExportOrder);
        Task<ResultModel> UpdateExportDetailAsync(int exportDetailId, UpdateExportDetail updateExportDetail);
        Task<ResultModel> DeleteExportOrderAsync(int exportOrderId);
        Task<ResultModel> DeleteExportDetailAsync(int exportDetailId);
    }
}
