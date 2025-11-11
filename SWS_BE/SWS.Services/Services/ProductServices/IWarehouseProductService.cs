using SWS.Services.ApiModels.ProductModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Dtos.Product;
using SWS.BusinessObjects.Models;
using SWS.Services.ApiModels.Commons;

namespace SWS.Services.Services.ProductServices
{

    public interface IWarehouseProductService
    {
        /// <summary>
        /// Lấy danh sách tất cả sản phẩm
        /// </summary>
        Task<ResultModel<IEnumerable<ProductResponseDto>>> GetAllProductsAsync();

        /// <summary>
        /// Lấy chi tiết sản phẩm theo Id
        /// </summary>
        Task<ResultModel<ProductResponseDto>> GetProductByIdAsync(int productId);

        /// <summary>
        /// Thêm mới sản phẩm
        /// </summary>
        Task<ResultModel> AddProductAsync(CreateProductRequest request);

        /// <summary>
        /// Cập nhật sản phẩm theo Id
        /// </summary>
        Task<ResultModel> UpdateProductAsync(int productId, UpdateProductRequest request);

        /// <summary>
        /// Xóa sản phẩm theo Id
        /// </summary>
        Task<ResultModel> DeleteProductAsync(int productId);

        /// <summary>
        /// Tìm sản phẩm sắp hết hạn(<30 ngày)
        /// </summary>
        Task<ResultModel<IEnumerable<ProductResponseDto>>> GetNearExpiredProductsAsync();

        /// <summary>
        /// Tìm sản phẩm đã hết hạn
        /// </summary>
        Task<ResultModel<IEnumerable<ProductResponseDto>>> GetExpiredProductsAsync();

        /// <summary>
        /// Tìm sản phẩm có ít hàng cần nhập thêm
        /// </summary>
        Task<ResultModel<IEnumerable<ProductResponseDto>>> GetLowStockProductsAsync();

        /// <summary>
        /// Tìm kiếm sản phẩm theo tên hoặc số serial
        /// </summary>
        Task<ResultModel<IEnumerable<ProductResponseDto>>> SearchProductsAsync(string searchText);
    }

}
