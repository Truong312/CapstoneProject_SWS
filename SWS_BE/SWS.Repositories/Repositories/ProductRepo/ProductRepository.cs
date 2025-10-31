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

        public async Task<IEnumerable<ProductListDto>> GetAllAsync(string? filter, string? sortBy, bool ascending)
        {
            try
            {
                var query = _context.Products.AsQueryable();
                //Get all product that have name or serial number contain filter
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query = query.Where(p =>
                    p.Name.Contains(filter) ||
                    p.SerialNumber.Contains(filter));
                }

                //sort by one property, case insensitive
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    var validColumns = typeof(Product)
                        .GetProperties()
                        .Select(p => p.Name.ToLower())
                        .ToHashSet();
                    if (validColumns.Contains(sortBy.ToLower()))
                    {
                        query = query.OrderBy($"{sortBy} {(ascending ? "ascending" : "descending")}");
                    }
                }

                var products = await query.ToListAsync();
                var result = products.Select(p => new ProductListDto
                {
                    ProductId = p.ProductId,
                    SerialNumber = p.SerialNumber,
                    Name = p.Name,
                    ExpiredDate = p.ExpiredDate,
                    ReceivedDate = p.ReceivedDate,
                    Unit = p.Unit,
                    UnitPrice = p.UnitPrice,
                    PurchasedPrice = p.PurchasedPrice,
                    ReorderPoint = p.ReorderPoint,
                    Image = p.Image,
                    Description = p.Description
                }).ToList();
                return result;
            }
            catch (Exception e)
            {
                return Enumerable.Empty<ProductListDto>();
            }
        }
        
    }
}
