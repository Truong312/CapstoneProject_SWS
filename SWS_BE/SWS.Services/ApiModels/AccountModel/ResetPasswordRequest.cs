using System.ComponentModel.DataAnnotations;

namespace SWS.Services.ApiModels;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Email không được để trống.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    public string Email { get; set; }
    public string? Password { get; set; }
}