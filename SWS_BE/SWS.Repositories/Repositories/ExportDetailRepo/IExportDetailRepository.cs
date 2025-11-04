using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.ExportDetailRepo
{
    public interface IExportDetailRepository: IGenericRepository<ExportDetail>
    {
        Task<IEnumerable<ExportDetail>> GetAllByExportOrderId(int exportOrderId);
    }
}
