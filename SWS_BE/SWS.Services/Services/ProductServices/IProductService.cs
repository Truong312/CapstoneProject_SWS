using SWS.Services.ApiModels;
using SWS.Services.ApiModels.ProductModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Dtos.Product;
using SWS.BusinessObjects.Models;

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
        }

}
