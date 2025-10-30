using Google;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


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
                return await _context.Products
                    .Where(p => p.ReorderPoint != null && p.ReorderPoint > 0)
                    .ToListAsync();
            }
        }
    }
