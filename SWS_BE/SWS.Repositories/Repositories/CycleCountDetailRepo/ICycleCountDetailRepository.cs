using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.CycleCountDetailRepo
{
    public interface ICycleCountDetailRepository: IGenericRepository<CycleCountDetail>
    {
        Task<IEnumerable<CycleCountDetail>> GetAllByCycleCountId(int cycleCountId);
        Task<CycleCountDetail> GetByCycleCountIdAndProductId(int cycleCountId, int productId);
        Task<CycleCountDetail> GetByCycleCountNameAndProductId(string cycleCountName, int productId);
    }
}
