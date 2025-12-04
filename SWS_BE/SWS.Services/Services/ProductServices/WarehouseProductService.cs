using Microsoft.AspNetCore.Http;
using SWS.BusinessObjects.Models;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.ProductModel;
using System.Linq;
using SWS.Services.Services.LogServices;
using System.Security.Claims;
using SWS.BusinessObjects.Enums;

namespace SWS.Services.Services.ProductServices
{
    public class WarehouseProductService : IWarehouseProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionLogService _actionLogService;
        public WarehouseProductService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IActionLogService actionLogService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _actionLogService = actionLogService;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new Exception("User is not authenticated.");
            }
            return int.Parse(userIdClaim);
        }

        /// <summary>
        /// Lấy danh sách tất cả sản phẩm
        /// </summary>
        public async Task<ResultModel<IEnumerable<ProductResponseDto>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();

                var productDtos = products.Select(p => new ProductResponseDto
                {
                    ProductId = p.ProductId,
                    SerialNumber = p.SerialNumber,
                    Name = p.Name,
                    ExpiredDate = p.ExpiredDate,
                    Unit = p.Unit,
                    UnitPrice = p.UnitPrice,
                    ReceivedDate = p.ReceivedDate,
                    PurchasedPrice = p.PurchasedPrice,
                    ReorderPoint = p.ReorderPoint,
                    Image = p.Image,
                    Description = p.Description
                });

                return new ResultModel<IEnumerable<ProductResponseDto>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách sản phẩm thành công",
                    Data = productDtos,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<IEnumerable<ProductResponseDto>>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lấy danh sách sản phẩm: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        /// <summary>
        /// Lấy chi tiết sản phẩm theo Id
        /// </summary>
        public async Task<ResultModel<ProductResponseDto>> GetProductByIdAsync(int productId)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                {
                    return new ResultModel<ProductResponseDto>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy sản phẩm",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                var productDto = new ProductResponseDto
                {
                    ProductId = product.ProductId,
                    SerialNumber = product.SerialNumber,
                    Name = product.Name,
                    ExpiredDate = product.ExpiredDate,
                    Unit = product.Unit,
                    UnitPrice = product.UnitPrice,
                    ReceivedDate = product.ReceivedDate,
                    PurchasedPrice = product.PurchasedPrice,
                    ReorderPoint = product.ReorderPoint,
                    Image = product.Image,
                    Description = product.Description
                };

                return new ResultModel<ProductResponseDto>
                {
                    IsSuccess = true,
                    Message = "Lấy chi tiết sản phẩm thành công",
                    Data = productDto,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<ProductResponseDto>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lấy sản phẩm: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        /// <summary>
        /// Thêm mới sản phẩm
        /// </summary>
        public async Task<ResultModel> AddProductAsync(CreateProductRequest request
        )
        {
            try
            {
                var product = new Product
                {
                    SerialNumber = request.SerialNumber,
                    Name = request.Name,
                    ExpiredDate = request.ExpiredDate,
                    Unit = request.Unit,
                    UnitPrice = request.UnitPrice,
                    ReceivedDate = request.ReceivedDate,
                    PurchasedPrice = request.PurchasedPrice,
                    ReorderPoint = request.ReorderPoint,
                    Image = request.Image,
                    Description = request.Description
                };

                await _unitOfWork.Products.AddAsync(product);
                await _actionLogService.CreateActionLogAsync(ActionType.Create, "Product", "Thêm sản phẩm mới");
                await _unitOfWork.SaveChangesAsync();

                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Thêm sản phẩm thành công",
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi thêm sản phẩm: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        public async Task<ResultModel> UpdateProductAsync(int productId, UpdateProductRequest request)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy sản phẩm để cập nhật",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                // Cập nhật nếu có giá trị mới
                if (!string.IsNullOrEmpty(request.Name)) product.Name = request.Name;
                if (request.ExpiredDate.HasValue) product.ExpiredDate = request.ExpiredDate.Value;
                if (!string.IsNullOrEmpty(request.Unit)) product.Unit = request.Unit;
                if (request.UnitPrice.HasValue) product.UnitPrice = request.UnitPrice.Value;
                if (request.ReceivedDate.HasValue) product.ReceivedDate = request.ReceivedDate.Value;
                if (request.PurchasedPrice.HasValue) product.PurchasedPrice = request.PurchasedPrice.Value;
                if (request.ReorderPoint.HasValue) product.ReorderPoint = request.ReorderPoint.Value;
                if (!string.IsNullOrEmpty(request.Image)) product.Image = request.Image;
                if (!string.IsNullOrEmpty(request.Description)) product.Description = request.Description;

                await _unitOfWork.Products.UpdateAsync(product);
                await _actionLogService.CreateActionLogAsync(ActionType.Update, "Product", "Cập nhật sản phẩm");
                await _unitOfWork.SaveChangesAsync();

                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Cập nhật sản phẩm thành công",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi cập nhật sản phẩm: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        /// <summary>
        /// Xóa sản phẩm theo Id
        /// </summary>
        public async Task<ResultModel> DeleteProductAsync(int productId)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy sản phẩm để xóa",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                await _unitOfWork.Products.DeleteAsync(product);
                await _actionLogService.CreateActionLogAsync(ActionType.Delete, "Product", "Xoá sản phẩm");
                await _unitOfWork.SaveChangesAsync();

                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Xóa sản phẩm thành công",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi xóa sản phẩm: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel<IEnumerable<ProductResponseDto>>> GetNearExpiredProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetNearExpiredProductsAsync(DateOnly.FromDateTime(DateTime.Now));
                var result = products.Select(product => new ProductResponseDto
                {
                    ProductId = product.ProductId,
                    SerialNumber = product.SerialNumber,
                    Name = product.Name,
                    ExpiredDate = product.ExpiredDate,
                    Unit = product.Unit,
                    UnitPrice = product.UnitPrice,
                    ReceivedDate = product.ReceivedDate,
                    PurchasedPrice = product.PurchasedPrice,
                    ReorderPoint = product.ReorderPoint,
                    Image = product.Image,
                    Description = product.Description
                });
                return new ResultModel<IEnumerable<ProductResponseDto>>
                {
                    IsSuccess = true,
                    Message = "Danh sách sản phẩm sắp hết hạn( dưới 30 ngày)",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<IEnumerable<ProductResponseDto>>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tìm sản phẩm sắp hết hạn: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel<IEnumerable<ProductResponseDto>>> GetExpiredProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetExpiredProductsAsync(DateOnly.FromDateTime(DateTime.Now));
                var result = products.Select(product => new ProductResponseDto
                {
                    ProductId = product.ProductId,
                    SerialNumber = product.SerialNumber,
                    Name = product.Name,
                    ExpiredDate = product.ExpiredDate,
                    Unit = product.Unit,
                    UnitPrice = product.UnitPrice,
                    ReceivedDate = product.ReceivedDate,
                    PurchasedPrice = product.PurchasedPrice,
                    ReorderPoint = product.ReorderPoint,
                    Image = product.Image,
                    Description = product.Description
                });
                return new ResultModel<IEnumerable<ProductResponseDto>>
                {
                    IsSuccess = true,
                    Message = "Danh sách sản phẩm đã hết hạn",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<IEnumerable<ProductResponseDto>>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tìm sản phẩm đã hết hạn: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel<IEnumerable<ProductResponseDto>>> GetLowStockProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetLowStockProductsAsync();
                var result = products.Select(product => new ProductResponseDto
                {
                    ProductId = product.ProductId,
                    SerialNumber = product.SerialNumber,
                    Name = product.Name,
                    ExpiredDate = product.ExpiredDate,
                    Unit = product.Unit,
                    UnitPrice = product.UnitPrice,
                    ReceivedDate = product.ReceivedDate,
                    PurchasedPrice = product.PurchasedPrice,
                    ReorderPoint = product.ReorderPoint,
                    Image = product.Image,
                    Description = product.Description
                });
                return new ResultModel<IEnumerable<ProductResponseDto>>
                {
                    IsSuccess = true,
                    Message = "Danh sách sản phẩm cần nhập thêm",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            }catch(Exception e)
            {
                return new ResultModel<IEnumerable<ProductResponseDto>>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tìm sản phẩm đã hết hạn: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        /// <summary>
        /// Tìm kiếm sản phẩm theo tên hoặc số serial
        /// </summary>
        public async Task<ResultModel<IEnumerable<ProductResponseDto>>> SearchProductsAsync(string searchText)
        {
            try
            {
                var products = await _unitOfWork.Products.SearchAsync(searchText);

                var result = products.Select(p => new ProductResponseDto
                {
                    ProductId = p.ProductId,
                    SerialNumber = p.SerialNumber,
                    Name = p.Name,
                    ExpiredDate = p.ExpiredDate,
                    Unit = p.Unit,
                    UnitPrice = p.UnitPrice,
                    ReceivedDate = p.ReceivedDate,
                    PurchasedPrice = p.PurchasedPrice,
                    ReorderPoint = p.ReorderPoint,
                    Image = p.Image,
                    Description = p.Description
                });

                return new ResultModel<IEnumerable<ProductResponseDto>>
                {
                    IsSuccess = true,
                    Message = $"Kết quả tìm kiếm cho từ khóa: '{searchText}'",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<IEnumerable<ProductResponseDto>>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tìm kiếm sản phẩm: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel<SWS.Services.ApiModels.Commons.PagedResponseDto<ProductResponseDto>>> GetProductsPagedAsync(PagedRequestDto req)
        {
            try
            {
                // Normalize paging
                var page = req?.Page >= 1 ? req.Page : 1;
                var pageSize = req?.PageSize >= 1 ? Math.Min(req.PageSize, 100) : 20; // cap pageSize to 100

                IEnumerable<Product> products;
                if (!string.IsNullOrWhiteSpace(req?.Q))
                {
                    products = await _unitOfWork.Products.SearchAsync(req.Q!);
                }
                else
                {
                    products = await _unitOfWork.Products.GetAllAsync();
                }

                var list = products.ToList();
                var total = list.Count;

                var items = list
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductResponseDto
                    {
                        ProductId = p.ProductId,
                        SerialNumber = p.SerialNumber,
                        Name = p.Name,
                        ExpiredDate = p.ExpiredDate,
                        Unit = p.Unit,
                        UnitPrice = p.UnitPrice,
                        ReceivedDate = p.ReceivedDate,
                        PurchasedPrice = p.PurchasedPrice,
                        ReorderPoint = p.ReorderPoint,
                        Image = p.Image,
                        Description = p.Description
                    })
                    .ToList();

                var paged = new PagedResponseDto<ProductResponseDto>
                {
                    Total = total,
                    Page = page,
                    PageSize = pageSize,
                    Items = items
                };

                return new ResultModel<PagedResponseDto<ProductResponseDto>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách sản phẩm theo phân trang thành công",
                    Data = paged,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<PagedResponseDto<ProductResponseDto>>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lấy sản phẩm theo phân trang: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}