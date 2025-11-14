using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SWS.BusinessObjects.Models;

namespace SWS.Repositories.Repositories.ReturnRepo
{
    public interface IReturnOrderCommandRepository
    {
        Task<ReturnOrder?> GetForUpdateAsync(int id, CancellationToken ct = default);
        Task AddActionLogAsync(ActionLog log, CancellationToken ct = default);
    }
}
