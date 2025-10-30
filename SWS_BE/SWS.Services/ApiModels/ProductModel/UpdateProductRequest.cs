using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Services.ApiModels.ProductModel
{
    public class UpdateProductRequest
    {
        /// <summary> Tên sản phẩm </summary>
        public string? Name { get; set; }

        /// <summary> Ngày hết hạn </summary>
        public DateOnly? ExpiredDate { get; set; }

        /// <summary> Đơn vị tính </summary>
        public string? Unit { get; set; }

        /// <summary> Giá bán </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary> Ngày nhập kho </summary>
        public DateOnly? ReceivedDate { get; set; }

        /// <summary> Giá mua vào </summary>
        public decimal? PurchasedPrice { get; set; }

        /// <summary> Điểm đặt hàng lại </summary>
        public int? ReorderPoint { get; set; }

        /// <summary> Đường dẫn ảnh sản phẩm </summary>
        public string? Image { get; set; }

        /// <summary> Mô tả sản phẩm </summary>
        public string? Description { get; set; }
    }
}
