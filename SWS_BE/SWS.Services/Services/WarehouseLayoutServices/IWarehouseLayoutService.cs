using SWS.BusinessObjects.Dtos.WarehouseLayout;
using SWS.Services.ApiModels.Commons;
using SWS.BusinessObjects.Dtos.WarehouseLayout;
using System.Threading.Tasks;

namespace SWS.Services.Services.WarehouseLayoutServices
{
    public interface IWarehouseLayoutService
    {
        /// <summary>
        /// Lấy layout kho 2D.
        /// Nếu truyền shelfId => chỉ 1 kệ
        /// Nếu truyền productId => chỉ những ô có sản phẩm đó
        /// </summary>
        Task<ResultModel<WarehouseLayoutResponse>> GetLayoutAsync(string? shelfId, int? productId);
    }
}
