using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWS.BusinessObjects.Dtos;
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
        private readonly IGoogleLoginService _googleLoginService;

        public WarehouseAuthController(
            IWarehouseAuthenticationService authService,
            IGoogleLoginService googleLoginService)
        {
            _authService = authService;
            _googleLoginService = googleLoginService;
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
        /// Lấy URL đăng nhập Google
        /// </summary>
        /// <returns>URL để redirect tới Google OAuth</returns>
        [HttpGet("google-url")]
        [AllowAnonymous]
        public IActionResult GetGoogleAuthUrl()
        {
            var result = _googleLoginService.GetGoogleLoginUrl();
            return Ok(new { 
                isSuccess = true, 
                message = "Lấy URL đăng nhập Google thành công",
                data = result 
            });
        }

        /// <summary>
        /// Callback endpoint từ Google OAuth (redirect về frontend với token)
        /// </summary>
        /// <param name="code">Authorization code từ Google</param>
        /// <param name="error">Error message nếu có</param>
        /// <returns>Redirect về frontend với token hoặc error</returns>
        [HttpGet("google-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback([FromQuery] string? code, [FromQuery] string? error)
        {
            // Nếu user từ chối hoặc có lỗi
            if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(code))
            {
                // Redirect về frontend với error
                return Redirect($"http://localhost:3000/auth/callback?error={error ?? "access_denied"}");
            }

            try
            {
                // Login với Google
                var result = await _authService.LoginWithGoogleAsync(code);
                
                if (result.IsSuccess && result.Data != null)
                {
                    // Redirect về frontend với token và user info
                    var token = result.Data.Token;
                    var isNewUser = result.Data.IsNewUser;
                    
                    return Redirect($"http://localhost:3000/auth/callback?token={token}&isNewUser={isNewUser}");
                }
                else
                {
                    // Redirect với error message
                    return Redirect($"http://localhost:3000/auth/callback?error={Uri.EscapeDataString(result.Message)}");
                }
            }
            catch (Exception ex)
            {
                // Redirect với error
                return Redirect($"http://localhost:3000/auth/callback?error={Uri.EscapeDataString(ex.Message)}");
            }
        }

        /// <summary>
        /// Đăng nhập bằng Google (dùng cho mobile app hoặc SPA)
        /// </summary>
        /// <param name="request">Mã code từ Google OAuth</param>
        /// <returns>Thông tin user và JWT token</returns>
        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(new { 
                    isSuccess = false,
                    message = "Mã code từ Google là bắt buộc" 
                });
            }

            var result = await _authService.LoginWithGoogleAsync(request.Code);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Call Api này sẽ tạo actionLog người dùng đã logout
        /// Chạy Api trên swagger sẽ không có tác dụng logout
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.LogoutAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
