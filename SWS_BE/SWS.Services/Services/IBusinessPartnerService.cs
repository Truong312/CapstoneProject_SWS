using SWS.BusinessObjects.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWS.Services.Services
{
    public interface IBusinessPartnerService
    {
        Task<IEnumerable<BusinessPartnerDto>> GetPartnersByTypeAsync(string partnerType);
    }
}

