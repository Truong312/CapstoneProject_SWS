using SWS.BusinessObjects.Models;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels.WarehouseUserModel;
using SWS.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using SWS.BusinessObjects.Dtos;
using SWS.Services.ApiModels.Commons;
using SWS.Services.Services.LogServices;
using SWS.BusinessObjects.Enums;

namespace SWS.Services.Services.WarehouseAuthentication
{
    public class WarehouseAuthenticationService : IWarehouseAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IGoogleLoginService _googleLoginService;
        private readonly IMapper _mapper;
        private readonly IActionLogService _actionLogService;

        public WarehouseAuthenticationService(
            IUnitOfWork unitOfWork, 
            IConfiguration configuration,
            IGoogleLoginService googleLoginService,
            IMapper mapper,
            IActionLogService actionLogService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _googleLoginService = googleLoginService;
            _mapper = mapper;
            _actionLogService = actionLogService;
        }

        public async Task<ResultModel> RegisterAsync(RegisterWarehouseRequest request)
        {
            try
            {
                // Check if email already exists
                var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Email đã tồn tại trong hệ thống",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                // Hash password
                var hashedPassword = PasswordHelper.HashPassword(request.Password);

                // Create new user
                var newUser = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    Password = hashedPassword,
                    Phone = request.Phone,
                    Address = request.Address,
                    Role = request.Role
                };

                await _unitOfWork.Users.AddAsync(newUser);
                await _actionLogService.CreateActionLogAsync(ActionType.Create, "User", "Thêm tài khoản mới");
                await _unitOfWork.SaveChangesAsync();

                // Generate JWT token
                var token = GenerateJwtToken(newUser);

                var userResponse = new UserResponseDto
                {
                    UserId = newUser.UserId,
                    FullName = newUser.FullName,
                    Email = newUser.Email,
                    Phone = newUser.Phone,
                    Address = newUser.Address,
                    Role = newUser.Role
                };

                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Đăng ký thành công",
                    Data = new
                    {
                        User = userResponse,
                        Token = token
                    },
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi đăng ký: {ex.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> LoginAsync(LoginWarehouseRequest request)
        {
            try
            {
                // Find user by email
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Email hoặc mật khẩu không chính xác",
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }

                // Verify password
                if (!PasswordHelper.VerifyPassword(request.Password, user.Password))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Email hoặc mật khẩu không chính xác",
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);

                var userResponse = new UserResponseDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role
                };
                await _actionLogService.CreateActionLogAsync(ActionType.Login, "User", $"User {userResponse.UserId} đăng nhập vào lúc {DateTime.Now}");
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Đăng nhập thành công",
                    Data = new
                    {
                        User = userResponse,
                        Token = token
                    },
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi đăng nhập: {ex.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                var userResponse = new UserResponseDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role
                };

                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Lấy thông tin người dùng thành công",
                    Data = userResponse,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lấy thông tin người dùng: {ex.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> UpdateUserAsync(int userId, RegisterWarehouseRequest request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                // Check if email is being changed and if it already exists
                if (user.Email != request.Email)
                {
                    var emailExists = await _unitOfWork.Users.IsEmailExistsAsync(request.Email);
                    if (emailExists)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            Message = "Email đã tồn tại trong hệ thống",
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }
                }

                // Update user information
                user.FullName = request.FullName;
                user.Email = request.Email;
                user.Phone = request.Phone;
                user.Address = request.Address;
                user.Role = request.Role;

                await _unitOfWork.Users.UpdateAsync(user);
                await _actionLogService.CreateActionLogAsync(ActionType.Update, "User", $"Cập nhật thông tin tài khoản{user.UserId}");
                await _unitOfWork.SaveChangesAsync();

                var userResponse = new UserResponseDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role
                };

                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Cập nhật thông tin người dùng thành công",
                    Data = userResponse,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi cập nhật thông tin người dùng: {ex.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                // Verify old password
                if (!PasswordHelper.VerifyPassword(oldPassword, user.Password))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu cũ không chính xác",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                if (oldPassword == newPassword)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu mới không được trùng với mật khẩu cũ",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                // Hash and update new password
                user.Password = PasswordHelper.HashPassword(newPassword);
                await _unitOfWork.Users.UpdateAsync(user);
                await _actionLogService.CreateActionLogAsync(ActionType.Update, "User", $"Cập nhật mật khẩu tài khoản id={user.UserId}");
                await _unitOfWork.SaveChangesAsync();

                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Đổi mật khẩu thành công",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi đổi mật khẩu: {ex.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }


        public async Task<ResultModel<GoogleLoginResponseDto>> LoginWithGoogleAsync(string code)
        {
            try
            {
                // Get user info from Google
                var googleUserInfo = await _googleLoginService.GetUserInfoFromCodeAsync(code);
                
                if (googleUserInfo == null || string.IsNullOrEmpty(googleUserInfo.Email))
                {
                    return new ResultModel<GoogleLoginResponseDto>
                    {
                        IsSuccess = false,
                        Message = "Không thể lấy thông tin từ Google",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                // Check if user already exists
                var existingUser = await _unitOfWork.Users.GetByEmailAsync(googleUserInfo.Email);
                
                User user;
                bool isNewUser = false;

                if (existingUser == null)
                {
                    // Create new user with Google info
                    user = new User
                    {
                        FullName = googleUserInfo.Name ?? googleUserInfo.Email,
                        Email = googleUserInfo.Email,
                        Password = PasswordHelper.HashPassword(Guid.NewGuid().ToString()), // Random password for Google users
                        Role = 0 // Default role
                    };

                    await _unitOfWork.Users.AddAsync(user);
                    await _unitOfWork.SaveChangesAsync();
                    isNewUser = true;
                }
                else
                {
                    user = existingUser;
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);

                // Map User entity to GoogleLoginResponseDto using AutoMapper
                var response = _mapper.Map<GoogleLoginResponseDto>(user);
                response.Token = token;
                response.IsNewUser = isNewUser;

                return new ResultModel<GoogleLoginResponseDto>
                {
                    IsSuccess = true,
                    Message = isNewUser ? "Đăng ký và đăng nhập bằng Google thành công" : "Đăng nhập bằng Google thành công",
                    Data = response,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<GoogleLoginResponseDto>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi đăng nhập bằng Google: {ex.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        private string GenerateJwtToken(User user)
        {
            // Đọc đúng cùng section với AuthConfig (Jwt)
            var issuer = _configuration["Jwt:Issuer"]!;
            var audience = _configuration["Jwt:Audience"]!;
            var keyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            var signingKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // Thêm cả "uid" lẫn NameIdentifier để mọi helper đều đọc được
            var claims = new List<Claim>
    {
        new("uid", user.UserId.ToString()),
        new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Name, user.FullName ?? string.Empty),
        new(ClaimTypes.Role, user.Role.ToString())
    };

            var minutes = int.TryParse(_configuration["Jwt:AccessTokenExpirationMinutes"], out var m) ? m : 30;

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ResultModel> LogoutAsync()
        {
            try
            {
                var userId = _actionLogService.GetCurrentUserId();
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = $"Không tìm thấy người dùng với Id={userId}",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                await _actionLogService.CreateActionLogAsync(ActionType.Logout, "User", $"User {user.UserId} đã logout vào lúc {DateTime.Now}");
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Đăng xuất thành công",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch(Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi xảy ra khi đăng xuất: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
