using AutoMapper;
using SWS.BusinessObjects.DTOs;
using SWS.Repositories.Repositories.BusinessPartners;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWS.Services.Services
{
    public class BusinessPartnerService : IBusinessPartnerService
    {
        private readonly IBusinessPartnerRepository _repository;
        private readonly IMapper _mapper;

        public BusinessPartnerService(IBusinessPartnerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BusinessPartnerDto>> GetPartnersByTypeAsync(string partnerType)
        {
            var partners = await _repository.GetPartnersByTypeAsync(partnerType);
            return _mapper.Map<IEnumerable<BusinessPartnerDto>>(partners);
        }
    }
}

