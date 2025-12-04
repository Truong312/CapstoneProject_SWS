using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.ExportRepo
{
    public interface IExportOrderRepository: IGenericRepository<ExportOrder>
    {
        Task<IEnumerable<ExportOrder>> GetAllByCustomerId(int customerId);
        Task<IEnumerable<ExportOrder>> GetShippedExportOrder();
        Task<IEnumerable<ExportOrder>> GetByStaff(int staffId);
        Task<IEnumerable<ExportDetail>> GetExportDetails(int exportOrderId);
        Task<IEnumerable<ExportOrder>> GetExportOrderByDate(DateOnly startDate, DateOnly endDate);
    }
}
