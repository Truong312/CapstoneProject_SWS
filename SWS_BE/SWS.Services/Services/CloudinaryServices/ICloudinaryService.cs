using SWS.BusinessObjects.Dtos;
using SWS.Services.ApiModels;

namespace SWS.Services;

public interface ICloudinaryService
{
    Task<ResultModel<CloudinaryUploadResponseDto>> UploadAsync(CloudinaryUploadRequestDto request);
    Task<ResultModel<CloudinaryDeleteResponseDto>> DeleteAsync(string publicId);
}