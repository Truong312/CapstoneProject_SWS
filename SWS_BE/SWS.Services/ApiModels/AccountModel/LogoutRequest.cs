using System.ComponentModel.DataAnnotations;

namespace SWS.Services.ApiModels
{
    public class LogoutRequest
    {
        [Required]
        public int AccountId { get; set; }
    }
}

