namespace SWS.Services.ApiModels.WarehouseUserModel
{
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int Role { get; set; }
    }
}

