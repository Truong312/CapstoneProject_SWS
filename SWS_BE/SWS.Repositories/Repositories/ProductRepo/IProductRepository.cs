using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Dtos.Product;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.ProductRepo
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        Task<IEnumerable<ProductListDto>> GetAllAsync(string? filter, string? sortBy, bool ascending);
    }
}
