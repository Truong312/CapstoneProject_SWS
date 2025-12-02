using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.WarehouseUserModel;
using SWS.Services.Services.WarehouseUserServices;

namespace SWS.ApiCore.Controllers
{
    [Authorize(Roles = "3")]
    [Route("api/admin/users")]
    [ApiController]
    public class WarehouseUserAdminController : BaseApiController
    {
        private readonly IWarehouseUserAdminService _userAdminService;

        public WarehouseUserAdminController(IWarehouseUserAdminService userAdminService)
        {
            _userAdminService = userAdminService;
        }

        /// <summary>
        /// User List (phân trang + search Q = tên/email)
        /// GET: api/admin/users/paged?page=1&pageSize=20&q=abc
        /// </summary>
        [HttpGet("paged")]
        public async Task<IActionResult> GetUsersPaged([FromQuery] PagedRequestDto req)
        {
            var result = await _userAdminService.GetUsersPagedAsync(req);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result.Data);
        }

        /// <summary>
        /// User Details cho admin
        /// GET: api/admin/users/5
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userAdminService.GetUserByIdAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result.Data);
        }

        /// <summary>
        /// Tạo user mới (full CRUD – Create)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterWarehouseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userAdminService.CreateUserAsync(request);
            if (!result.IsSuccess)
                return BadRequest(result);

            // Có thể dùng CreatedAtAction nếu muốn
            return Ok(result.Data);
        }

        /// <summary>
        /// Cập nhật user (full CRUD – Update)
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] RegisterWarehouseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userAdminService.UpdateUserAsync(id, request);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result.Data);
        }

       
    }
}
