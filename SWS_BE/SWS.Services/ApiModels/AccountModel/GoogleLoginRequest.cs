using System.ComponentModel.DataAnnotations;

namespace SWS.Services.ApiModels
{
    public class GoogleLoginRequest
    {
        [Required]
        public string IdToken { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        public string? Name { get; set; }
        public string? Picture { get; set; }
    }
}
