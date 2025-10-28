using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWS.Services.ApiModels.WarehouseUserModel;
using SWS.Services.Services.WarehouseAuthentication;
using System.Security.Claims;

namespace SWS.ApiCore.Controllers
{
    /// <summary>
    /// API quản lý xác thực cho hệ thống SmartWarehouse
    /// </summary>
    [Route("api/warehouse/auth")]
    [ApiController]
    public class WarehouseAuthController : BaseApiController
    {
        private readonly IWarehouseAuthenticationService _authService;

        public WarehouseAuthController(IWarehouseAuthenticationService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        /// <param name="request">Thông tin đăng ký</param>
        /// <returns>Thông tin user và JWT token</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterWarehouseRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result.IsSuccess)
                return BadRequest(result);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="request">Email và mật khẩu</param>
        /// <returns>Thông tin user và JWT token</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginWarehouseRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin user hiện tại
        /// </summary>
        /// <returns>Thông tin user</returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { Message = "Token không hợp lệ" });
            }

            var result = await _authService.GetUserByIdAsync(userId);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin user theo ID
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <returns>Thông tin user</returns>
        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var result = await _authService.GetUserByIdAsync(userId);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Cập nhật thông tin user
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Thông tin user sau khi cập nhật</returns>
        [HttpPut("{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] RegisterWarehouseRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized(new { Message = "Token không hợp lệ" });
            }

            // Only allow users to update their own profile or admin can update anyone
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentUserId != userId && userRole != "1") // Assuming Role 1 is Admin
            {
                return Forbid();
            }

            var result = await _authService.UpdateUserAsync(userId, request);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="request">Mật khẩu cũ và mật khẩu mới</param>
        /// <returns>Kết quả đổi mật khẩu</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { Message = "Token không hợp lệ" });
            }

            var result = await _authService.ChangePasswordAsync(userId, request.OldPassword, request.NewPassword);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Đăng nhập bằng Google
        /// </summary>
        /// <param name="request">Mã code từ Google OAuth</param>
        /// <returns>Thông tin user và JWT token</returns>
        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(new { Message = "Mã code từ Google là bắt buộc" });
            }

            var result = await _authService.LoginWithGoogleAsync(request.Code);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    public class GoogleLoginRequest
    {
        public string Code { get; set; } = null!;
    }
}
