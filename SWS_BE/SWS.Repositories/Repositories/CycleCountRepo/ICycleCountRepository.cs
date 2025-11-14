using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.CycleCountRepo
{
    public interface ICycleCountRepository: IGenericRepository<CycleCount>
    {
        Task<CycleCount>GetByName(string name); 
    }
}
