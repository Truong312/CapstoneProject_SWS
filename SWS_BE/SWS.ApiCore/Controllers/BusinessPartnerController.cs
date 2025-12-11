using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWS.BusinessObjects.DTOs;
using SWS.BusinessObjects.Enums;
using SWS.Services.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWS.ApiCore.Controllers
{
    [ApiController]
    [Route("api/business-partners")]
    public class BusinessPartnerController : ControllerBase
    {
        private readonly IBusinessPartnerService _service;

        public BusinessPartnerController(IBusinessPartnerService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách các nhà cung cấp (Providers).
        /// </summary>
        [HttpGet("providers")]
        public async Task<ActionResult<IEnumerable<BusinessPartnerDto>>> GetProviders()
        {
            var providers = await _service.GetPartnersByTypeAsync(PartnerType.Supplier.ToString());
            return Ok(providers);
        }
    }
}
