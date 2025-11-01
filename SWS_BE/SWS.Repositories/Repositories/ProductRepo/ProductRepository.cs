using Google;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Dtos.Product;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;
using System.Linq.Dynamic.Core;


namespace SWS.Repositories.Repositories.ProductRepo
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {

        private readonly SmartWarehouseDbContext _context;

        public ProductRepository(SmartWarehouseDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product?> GetBySerialNumberAsync(string serialNumber)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.SerialNumber == serialNumber);
        }

        public async Task<IEnumerable<Product>> GetNearExpiredProductsAsync(DateOnly currentDate)
        {
            return await _context.Products
                .Where(p => p.ExpiredDate < currentDate.AddDays(30))
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetExpiredProductsAsync(DateOnly currentDate)
        {
            return await _context.Products
                .Where(p => p.ExpiredDate < currentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            // Giả sử ReorderPoint là mức tồn kho tối thiểu
            // và bạn muốn tìm các sản phẩm có tồn kho thấp hơn mức đó.
            var products = await _context.Products.ToListAsync();
            var result = new List<Product>();
            foreach (var product in products)
            {
                //quantity là số lượng sản có thể tham gia order 
                var quantity = _context.Inventories.Where(i => i.ProductId == product.ProductId).FirstAsync().Result.QuantityAvailable;
                if(quantity< product.ReorderPoint)
                {
                    result.Add(product);
                }
            }
            return result;
        }
    }
}