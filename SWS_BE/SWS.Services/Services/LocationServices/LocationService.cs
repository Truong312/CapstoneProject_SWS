using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SWS.BusinessObjects.Models;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.LocationModel;

namespace SWS.Services.Services.LocationServices
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public LocationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultModel<IEnumerable<LocationResponse>>> GetAllLocationAsync()
        {
            try
            {
                var locations = await _unitOfWork.Locations.GetAllAsync();
                if (locations == null)
                {
                    return new ResultModel<IEnumerable<LocationResponse>>
                    {
                        IsSuccess = false,
                        Message = "Không tìm được location nào",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var result = locations.Select(l => new LocationResponse
                {
                    LocationId = l.LocationId,
                    ShelfId = l.ShelfId,
                    ColumnNumber = l.ColumnNumber,
                    RowNumber = l.RowNumber,
                    Type = l.Type,
                    IsFull = l.IsFull
                });
                return new ResultModel<IEnumerable<LocationResponse>>
                {
                    IsSuccess = true,
                    Message = "Danh sách location trong kho",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<IEnumerable<LocationResponse>>
                {
                    IsSuccess = true,
                    Message = $"Lỗi xảy ra khi lấy danh sách location: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel<IEnumerable<LocationResponse>>> GetAllLocationWithProductIdAsync(int productId)
        {
            try
            {
                var locations = await _unitOfWork.Locations.GetByProductId(productId);
                if (locations == null)
                {
                    return new ResultModel<IEnumerable<LocationResponse>>
                    {
                        IsSuccess = false,
                        Message = $"Không tìm được location nào với productId={productId}",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var result = locations.Select(l => new LocationResponse
                {
                    LocationId = l.LocationId,
                    ShelfId = l.ShelfId,
                    ColumnNumber = l.ColumnNumber,
                    RowNumber = l.RowNumber,
                    Type = l.Type,
                    IsFull = l.IsFull
                });
                return new ResultModel<IEnumerable<LocationResponse>>
                {
                    IsSuccess = true,
                    Message = $"Danh sách location trong kho đang chứa product với productId={productId}",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<IEnumerable<LocationResponse>>
                {
                    IsSuccess = true,
                    Message = $"Lỗi xảy ra khi lấy danh sách location với productId={productId}: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel<LocationResponse>> GetLocationByIdAsync(int locationId)
        {
            try
            {
                var location = await _unitOfWork.Locations.GetByIdAsync(locationId);
                if (location == null)
                {
                    return new ResultModel<LocationResponse>
                    {
                        IsSuccess = false,
                        Message = $"Không tìm được location nào với id={locationId}",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var result = new LocationResponse
                {
                    LocationId = location.LocationId,
                    ShelfId = location.ShelfId,
                    ColumnNumber = location.ColumnNumber,
                    RowNumber = location.RowNumber,
                    Type = location.Type,
                    IsFull = location.IsFull
                };
                return new ResultModel<LocationResponse>
                {
                    IsSuccess = true,
                    Message = $"Location với id={locationId}",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<LocationResponse>
                {
                    IsSuccess = true,
                    Message = $"Lỗi xảy ra khi lấy location với id={locationId}: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> CreateNewLocationAsync(CreateLocation createLocation)
        {
            try
            {
                var location = new Location
                {
                    ShelfId = createLocation.ShelfId,
                    ColumnNumber = createLocation.ColumnNumber,
                    RowNumber = createLocation.RowNumber,
                    Type = createLocation.Type,
                    IsFull = createLocation.IsFull
                };
                await _unitOfWork.Locations.AddAsync(location);
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = $"Đã thêm location:{location}",
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi xảy ra khi tạo location: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> UpdateLocationAsync(int locationId, UpdateLocation updateLocation)
        {
            try
            {
                var location = await _unitOfWork.Locations.GetByIdAsync(locationId);
                if (location == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = $"Không tìm được location với id={locationId}",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                if (location.ShelfId != updateLocation.ShelfId) location.ShelfId = updateLocation.ShelfId;
                if (location.ColumnNumber != updateLocation.ColumnNumber) location.ColumnNumber = updateLocation.ColumnNumber;
                if (location.RowNumber != updateLocation.RowNumber) location.RowNumber = updateLocation.RowNumber;
                if (location.Type != updateLocation.Type) location.Type = updateLocation.Type;
                if (location.IsFull != updateLocation.IsFull) location.IsFull = updateLocation.IsFull;
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = $"Đã cập nhật thành công location với id={locationId}",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi xảy ra khi cập nhật location với id={locationId}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> DeleteLocationAsync(int locationId)
        {
            try
            {
                var location = await _unitOfWork.Locations.GetByIdAsync(locationId);
                if (location == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = $"Không tìm được location với id={locationId}",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                await _unitOfWork.Locations.DeleteByIdAsync(locationId);
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = $"Đã xóa location với id={locationId}",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi xảy ra khi xóa location với id={locationId}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
