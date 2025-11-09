using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.CycleCountRepo
{
   public class CycleCountRepository: GenericRepository<CycleCount>, ICycleCountRepository
    {
        private readonly SmartWarehouseDbContext _context;

        public CycleCountRepository(SmartWarehouseDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
