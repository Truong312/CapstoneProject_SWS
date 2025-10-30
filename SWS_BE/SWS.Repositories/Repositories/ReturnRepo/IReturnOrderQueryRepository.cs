using SWS.BusinessObjects.Models;

namespace SWS.Repositories.Repositories.ReturnRepo
{
    public interface IReturnOrderQueryRepository
    {
        Task<IEnumerable<ReturnOrder>> GetListAsync(
            DateOnly? from, DateOnly? to, string? status,
            int? exportOrderId, int? checkedBy, int? reviewedBy);

        Task<ReturnOrder?> GetDetailAsync(int id);
    }
}
