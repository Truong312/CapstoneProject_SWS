using System.Threading;
using System.Threading.Tasks;
using SWS.BusinessObjects.DTOs;
using SWS.BusinessObjects.Models;

namespace SWS.Repositories.Repositories.ReturnRepo
{
    public interface IReturnOrderCommandRepository
    {
        // Dùng cho REVIEW 
        Task<ReturnOrder?> GetForUpdateAsync(int id, CancellationToken ct = default);
        Task AddActionLogAsync(ActionLog log, CancellationToken ct = default);

        //  Dùng cho CREATE
        Task<CreateReturnOrderResponse> CreateAsync(
            int createdBy,
            CreateReturnOrderRequest req,
            CancellationToken ct = default);
    }
}
