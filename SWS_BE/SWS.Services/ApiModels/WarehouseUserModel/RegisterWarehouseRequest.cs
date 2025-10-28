using System.ComponentModel.DataAnnotations;

namespace SWS.Services.ApiModels.WarehouseUserModel
{
    public class RegisterWarehouseRequest
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public int Role { get; set; } = 0; // Default role
    }
}

