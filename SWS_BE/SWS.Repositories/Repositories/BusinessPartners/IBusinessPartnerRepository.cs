using SWS.BusinessObjects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWS.Repositories.Repositories.BusinessPartners
{
    public interface IBusinessPartnerRepository
    {
        Task<IEnumerable<BusinessPartner>> GetPartnersByTypeAsync(string partnerType);
    }
}

