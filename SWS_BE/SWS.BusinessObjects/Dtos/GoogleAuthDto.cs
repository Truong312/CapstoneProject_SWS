using System.ComponentModel.DataAnnotations;

namespace SWS.BusinessObjects.Dtos
{
    /// <summary>
    /// DTO cho request đăng nhập bằng Google
    /// </summary>
    public class GoogleLoginRequestDto
    {
        [Required(ErrorMessage = "Authorization code là bắt buộc")]
        public string Code { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho thông tin user từ Google
    /// </summary>
    public class GoogleUserInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool VerifiedEmail { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GivenName { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public string Locale { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho response sau khi đăng nhập thành công
    /// </summary>
    public class GoogleLoginResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int Role { get; set; }
        public string Token { get; set; } = string.Empty;
        public bool IsNewUser { get; set; }
    }

    /// <summary>
    /// DTO cho URL đăng nhập Google
    /// </summary>
    public class GoogleAuthUrlDto
    {
        public string AuthUrl { get; set; } = string.Empty;
    }
}

