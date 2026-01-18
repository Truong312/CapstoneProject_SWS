using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWS.BusinessObjects.Dtos;
using SWS.Services.Services.UserServices;

namespace SWS.ApiCore.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get paginated list of users with filtering and sorting
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "1,2")] // Admin and Manager only
        public async Task<IActionResult> GetPagedUsers([FromQuery] UserPagedRequestDto request)
        {
            var result = await _userService.GetPagedUsersAsync(request);
            
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "1,2")] // Admin and Manager only
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "1")] // Admin only
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.CreateUserAsync(request);
            
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "1")] // Admin only
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateUserAsync(id, request);
            
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "1")] // Admin only
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get providers/suppliers (Role = 4)
        /// </summary>
        [HttpGet("providers")]
        public async Task<IActionResult> GetProviders([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var providerRequest = new UserPagedRequestDto
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                RoleFilter = 4 // Provider/Supplier role
            };

            var result = await _userService.GetPagedUsersAsync(providerRequest);
            
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
