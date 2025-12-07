using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SWS.BusinessObjects.Models;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.WarehouseUserModel;
using SWS.Services.Helpers;

namespace SWS.Services.Services.WarehouseUserServices
{
    public class WarehouseUserAdminService : IWarehouseUserAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseUserAdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultModel<PagedResponseDto<UserResponseDto>>> GetUsersPagedAsync(PagedRequestDto req)
        {
            try
            {
                var page = req?.Page >= 1 ? req.Page : 1;
                var pageSize = req?.PageSize >= 1 ? Math.Min(req.PageSize, 100) : 20;

                var keyword = req?.Q?.Trim();
                IEnumerable<User> users;

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    // search theo FullName hoặc Email
                    users = await _unitOfWork.Users.FindAsync(u =>
                        u.FullName.Contains(keyword) || u.Email.Contains(keyword));
                }
                else
                {
                    users = await _unitOfWork.Users.GetAllAsync();
                }

                var list = users
                    .OrderBy(u => u.UserId)
                    .ToList();

                var total = list.Count;

                var items = list
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserResponseDto
                    {
                        UserId = u.UserId,
                        FullName = u.FullName,
                        Email = u.Email,
                        Phone = u.Phone,
                        Address = u.Address,
                        Role = u.Role
                    })
                    .ToList();

                var paged = new PagedResponseDto<UserResponseDto>
                {
                    Total = total,
                    Page = page,
                    PageSize = pageSize,
                    Items = items
                };

                return new ResultModel<PagedResponseDto<UserResponseDto>>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy danh sách user theo phân trang thành công",
                    Data = paged
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<PagedResponseDto<UserResponseDto>>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi khi lấy danh sách user: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<UserResponseDto>> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ResultModel<UserResponseDto>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Không tìm thấy người dùng"
                    };
                }

                var dto = new UserResponseDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role
                };

                return new ResultModel<UserResponseDto>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy thông tin user thành công",
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<UserResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi khi lấy thông tin user: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<UserResponseDto>> CreateUserAsync(RegisterWarehouseRequest request)
        {
            try
            {
                // Check email trùng
                var emailExists = await _unitOfWork.Users.IsEmailExistsAsync(request.Email);
                if (emailExists)
                {
                    return new ResultModel<UserResponseDto>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Email đã tồn tại trong hệ thống"
                    };
                }

                // Hash password
                var hashedPassword = PasswordHelper.HashPassword(request.Password);

                var newUser = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    Password = hashedPassword,
                    Phone = request.Phone,
                    Address = request.Address,
                    Role = request.Role   // admin có thể set Role
                };

                await _unitOfWork.Users.AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();

                var dto = new UserResponseDto
                {
                    UserId = newUser.UserId,
                    FullName = newUser.FullName,
                    Email = newUser.Email,
                    Phone = newUser.Phone,
                    Address = newUser.Address,
                    Role = newUser.Role
                };

                return new ResultModel<UserResponseDto>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Tạo user mới thành công",
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<UserResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi khi tạo user: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<UserResponseDto>> UpdateUserAsync(int userId, RegisterWarehouseRequest request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ResultModel<UserResponseDto>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Không tìm thấy người dùng"
                    };
                }

                // Nếu đổi email thì check trùng
                if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailExists = await _unitOfWork.Users.IsEmailExistsAsync(request.Email);
                    if (emailExists)
                    {
                        return new ResultModel<UserResponseDto>
                        {
                            IsSuccess = false,
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = "Email đã tồn tại trong hệ thống"
                        };
                    }
                }

                // Update thông tin
                user.FullName = request.FullName;
                user.Email = request.Email;
                user.Phone = request.Phone;
                user.Address = request.Address;
                user.Role = request.Role;

                // (tuỳ bạn) nếu muốn cho admin đổi mật khẩu luôn:
                // nếu request.Password không rỗng thì hash lại:
                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    user.Password = PasswordHelper.HashPassword(request.Password);
                }

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var dto = new UserResponseDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role
                };

                return new ResultModel<UserResponseDto>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Cập nhật user thành công",
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<UserResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi khi cập nhật user: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<bool>> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ResultModel<bool>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Không tìm thấy người dùng",
                        Data = false
                    };
                }

                await _unitOfWork.Users.DeleteAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return new ResultModel<bool>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Xoá user thành công",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<bool>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi khi xoá user: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
