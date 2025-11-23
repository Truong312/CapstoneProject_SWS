using System.Collections.Generic;
using System.Threading.Tasks;
using SWS.Services.ApiModels;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.WarehouseUserModel;

namespace SWS.Services.Services.WarehouseUserServices
{
    /// <summary>
    /// Service dành riêng cho admin/staff quản lý user (CRUD, list, search)
    /// </summary>
    public interface IWarehouseUserAdminService
    {
        /// <summary>
        /// Lấy danh sách user theo phân trang + search (FullName / Email)
        /// </summary>
        Task<ResultModel<PagedResponseDto<UserResponseDto>>> GetUsersPagedAsync(PagedRequestDto req);

        /// <summary>
        /// Lấy chi tiết 1 user theo Id (admin xem hồ sơ)
        /// </summary>
        Task<ResultModel<UserResponseDto>> GetUserByIdAsync(int userId);

        /// <summary>
        /// Admin tạo user mới (staff nội bộ)
        /// </summary>
        Task<ResultModel<UserResponseDto>> CreateUserAsync(RegisterWarehouseRequest request);

        /// <summary>
        /// Admin cập nhật thông tin user (bao gồm Role)
        /// </summary>
        Task<ResultModel<UserResponseDto>> UpdateUserAsync(int userId, RegisterWarehouseRequest request);

        /// <summary>
        /// Admin xoá user (hard delete, vì bảng User không có cờ IsActive/IsDeleted)
        /// </summary>
        Task<ResultModel<bool>> DeleteUserAsync(int userId);
    }
}
