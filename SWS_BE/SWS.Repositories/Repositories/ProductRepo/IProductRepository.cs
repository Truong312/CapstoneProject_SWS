using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Repositories.Repositories.ProductRepo
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // Có thể thêm các hàm đặc thù nếu cần
        Task<Product?> GetBySerialNumberAsync(string serialNumber);
        Task<IEnumerable<Product>> GetExpiredProductsAsync(DateOnly currentDate);
        Task<IEnumerable<Product>> GetLowStockProductsAsync();
    }
}
