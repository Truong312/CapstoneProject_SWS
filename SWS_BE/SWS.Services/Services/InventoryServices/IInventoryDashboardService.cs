using SWS.BusinessObjects.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Services.Services.InventoryServices
{
    public interface IInventoryDashboardService
    {
        Task<InventoryDashboardDto> GetDashboardAsync();
    }

}
