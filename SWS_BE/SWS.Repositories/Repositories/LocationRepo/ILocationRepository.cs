using System.Collections.Generic;
using System.Threading.Tasks;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.LocationRepo
{
    public interface ILocationRepository : IGenericRepository<Location>
    {
        /// <summary>
        /// Lấy danh sách Location + Inventory + Product
        /// Nếu truyền shelfId => lọc theo kệ
        /// Nếu truyền productId => chỉ lấy các ô có sản phẩm đó
        /// </summary>
        Task<IEnumerable<Location>> GetLayoutAsync(string? shelfId, int? productId);
        Task<IEnumerable<Location>> GetByProductId(int productId);
    }
}
