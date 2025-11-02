using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Models;
using SWS.Services.ApiModels;
using SWS.Services.ApiModels.ExportOrderModel;

namespace SWS.Services.Services.ExportOrderServices
{
    public interface IExportOrderService
    {
        Task<ResultModel<IEnumerable<ExportOrderResponse>>> GetAllExportOrdersAsync();
        Task<ResultModel<IEnumerable<ExportDetailResponse>>> GetExportOrderDetails(int exportOrderId);
    }
}
