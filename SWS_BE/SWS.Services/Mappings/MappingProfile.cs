using AutoMapper;
using SWS.BusinessObjects.Dtos;
using SWS.BusinessObjects.DTOs;
using SWS.BusinessObjects.Models;
using SWS.Services.ApiModels;

namespace SWS.Services.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BusinessPartner, BusinessPartnerDto>();
            #region AccountServices
            #endregion

            #region Account
            #endregion

            #region Google Authentication
            // Map từ GoogleUserInfo (ApiModel) sang GoogleUserInfoDto (DTO)
            CreateMap<GoogleUserInfo, GoogleUserInfoDto>();
            
            // Map từ User entity sang GoogleLoginResponseDto
            CreateMap<User, GoogleLoginResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.IsNewUser, opt => opt.Ignore());
            #endregion

            #region User Management
            // Map User entity to UserDto
            CreateMap<User, SWS.BusinessObjects.Dtos.UserDto>()
                .ForMember(dest => dest.RoleName, opt => opt.Ignore());

            // Map CreateUserRequest to User entity
            CreateMap<CreateUserRequest, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore()); // Password will be hashed separately

            // Map UpdateUserRequest to User entity
            CreateMap<UpdateUserRequest, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore());
            #endregion
        }
    }
}