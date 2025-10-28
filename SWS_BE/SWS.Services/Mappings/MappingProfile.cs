using AutoMapper;
using SWS.BusinessObjects.Dtos;
using SWS.BusinessObjects.Models;
using SWS.Services.ApiModels;

namespace SWS.Services.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
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
        }
    }
}