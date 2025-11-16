using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.LocationRepo
{
    public interface ILocationRepository:IGenericRepository<Location>
    {
        Task<IEnumerable<Location>> GetByProductId(int productId);
    }
}
