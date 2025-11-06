using SWS.BusinessObjects.DTOs;
using SWS.Repositories.UnitOfWork;

namespace SWS.Services.ReturnLookups
{
    public interface IReturnLookupService
    {
        Task<List<ReturnReasonDto>> GetReasonsAsync(string? q);
        Task<List<ReturnStatusDto>> GetStatusesAsync(string? q);
    }

    public class ReturnLookupService : IReturnLookupService
    {
        private readonly IUnitOfWork _uow;
        public ReturnLookupService(IUnitOfWork uow) => _uow = uow;

        public async Task<List<ReturnReasonDto>> GetReasonsAsync(string? q)
        {
            var reasons = await _uow.ReturnReasons.SearchAsync(q);
            return reasons
                .Select(r => new ReturnReasonDto(r.ReasonId, r.ReasonCode ?? string.Empty, r.Description ?? string.Empty))
                .ToList();
        }

        public Task<List<ReturnStatusDto>> GetStatusesAsync(string? q)
            => _uow.ReturnStatuses.SearchAsync(q);
    }
}
