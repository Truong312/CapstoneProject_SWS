using SWS.BusinessObjects.Dtos;
using SWS.Services.ApiModels.Commons;

namespace SWS.Services.Services.UserServices
{
    public interface IUserService
    {
        Task<ResultModel<SWS.Services.ApiModels.Commons.PagedResponseDto<UserDto>>> GetPagedUsersAsync(UserPagedRequestDto request);
        Task<ResultModel<UserDto>> GetUserByIdAsync(int userId);
        Task<ResultModel<UserDto>> CreateUserAsync(CreateUserRequest request);
        Task<ResultModel<UserDto>> UpdateUserAsync(int userId, UpdateUserRequest request);
        Task<ResultModel> DeleteUserAsync(int userId);
    }
}
