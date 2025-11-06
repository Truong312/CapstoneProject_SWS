using SWS.BusinessObjects.DTOs;

namespace SWS.Repositories.Repositories.ReturnRepo
{
    public interface IReturnStatusRepository
    {
        // DISTINCT status + count từ ReturnOrder, có q để filter theo chuỗi
        Task<List<ReturnStatusDto>> SearchAsync(string? q);
    }
}
