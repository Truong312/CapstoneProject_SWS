using SWS.BusinessObjects.Models;

namespace SWS.Repositories.Repositories.ReturnRepo
{
    public interface IReturnReasonRepository
    {
        Task<IEnumerable<ReturnReason>> SearchAsync(string? q);
    }
}
