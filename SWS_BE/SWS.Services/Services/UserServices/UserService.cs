using AutoMapper;
using SWS.BusinessObjects.Dtos;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Repositories.UserRepo;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels.Commons;

namespace SWS.Services.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultModel<SWS.Services.ApiModels.Commons.PagedResponseDto<UserDto>>> GetPagedUsersAsync(UserPagedRequestDto request)
        {
            try
            {
                var (items, totalCount) = await _userRepository.GetPagedUsersAsync(
                    request.PageIndex,
                    request.PageSize,
                    request.Search,
                    request.RoleFilter,
                    request.SortBy,
                    request.SortDesc
                );

                var userDtos = _mapper.Map<List<UserDto>>(items);

                // Map role numbers to role names
                foreach (var user in userDtos)
                {
                    user.RoleName = GetRoleName(user.Role);
                }

                var pagedResponse = new SWS.Services.ApiModels.Commons.PagedResponseDto<UserDto>
                {
                    Items = userDtos,
                    Page = request.PageIndex,
                    PageSize = request.PageSize,
                    Total = (int)Math.Ceiling(totalCount / (double)request.PageSize)
                };

                return new ResultModel<SWS.Services.ApiModels.Commons.PagedResponseDto<UserDto>>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Data = pagedResponse,
                    Message = "Users retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<SWS.Services.ApiModels.Commons.PagedResponseDto<UserDto>>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    ResponseCode = "ERROR",
                    Message = $"Error retrieving users: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<UserDto>> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return new ResultModel<UserDto>
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        ResponseCode = "NOT_FOUND",
                        Message = "User not found"
                    };
                }

                var userDto = _mapper.Map<UserDto>(user);
                userDto.RoleName = GetRoleName(userDto.Role);

                return new ResultModel<UserDto>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Data = userDto,
                    Message = "User retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<UserDto>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    ResponseCode = "ERROR",
                    Message = $"Error retrieving user: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<UserDto>> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                // Check if email already exists
                if (await _userRepository.IsEmailExistsAsync(request.Email))
                {
                    return new ResultModel<UserDto>
                    {
                        IsSuccess = false,
                        StatusCode = 409,
                        ResponseCode = "CONFLICT",
                        Message = "Email already exists"
                    };
                }

                var user = _mapper.Map<User>(request);
                
                // Hash password (you should use proper password hashing in production)
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userDto = _mapper.Map<UserDto>(user);
                userDto.RoleName = GetRoleName(userDto.Role);

                return new ResultModel<UserDto>
                {
                    IsSuccess = true,
                    StatusCode = 201,
                    Data = userDto,
                    Message = "User created successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<UserDto>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    ResponseCode = "ERROR",
                    Message = $"Error creating user: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<UserDto>> UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return new ResultModel<UserDto>
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        ResponseCode = "NOT_FOUND",
                        Message = "User not found"
                    };
                }

                // Update user properties
                user.FullName = request.FullName;
                user.Phone = request.Phone;
                user.Address = request.Address;
                user.Role = request.Role;

                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userDto = _mapper.Map<UserDto>(user);
                userDto.RoleName = GetRoleName(userDto.Role);

                return new ResultModel<UserDto>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Data = userDto,
                    Message = "User updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<UserDto>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    ResponseCode = "ERROR",
                    Message = $"Error updating user: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        ResponseCode = "NOT_FOUND",
                        Message = "User not found"
                    };
                }

                await _userRepository.DeleteAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "User deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    ResponseCode = "ERROR",
                    Message = $"Error deleting user: {ex.Message}"
                };
            }
        }

        private string GetRoleName(int role)
        {
            return role switch
            {
                1 => "Admin",
                2 => "Manager",
                3 => "Warehouse Staff",
                4 => "Provider/Supplier",
                _ => "Unknown"
            };
        }
    }
}
