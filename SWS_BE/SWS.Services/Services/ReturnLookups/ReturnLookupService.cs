using SWS.BusinessObjects.DTOs;
using SWS.Repositories.Repositories.ReturnRepo;

namespace SWS.Services.ReturnLookups
{
    public interface IReturnLookupService
    {
        Task<List<ReturnReasonDto>> GetReasonsAsync(string? q);
        Task<List<ReturnStatusDto>> GetStatusesAsync(string? q);
    }

    public class ReturnLookupService : IReturnLookupService
    {
        private readonly IReturnReasonRepository _reasonRepo;
        private readonly IReturnStatusRepository _statusRepo;

        public ReturnLookupService(
            IReturnReasonRepository reasonRepo,
            IReturnStatusRepository statusRepo)
        {
            _reasonRepo = reasonRepo;
            _statusRepo = statusRepo;
        }

        public async Task<List<ReturnReasonDto>> GetReasonsAsync(string? q)
        {
            var items = await _reasonRepo.SearchAsync(q);
            return items.Select(r => new ReturnReasonDto(r.ReasonId, r.ReasonCode, r.Description)).ToList();
        }

        public Task<List<ReturnStatusDto>> GetStatusesAsync(string? q)
            => _statusRepo.SearchAsync(q);
    }
}
