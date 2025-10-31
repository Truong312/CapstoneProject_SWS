using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Dtos.Product;
using SWS.BusinessObjects.Models;

namespace SWS.Services.Services.ProductServices
{
    public interface IProductService
    {
        Task<Product?> CreateProductAsync(CreateProductDto createProductDto);
        Task<Product?> UpdateProductAsync(UpdateProductDto updateProductDto);
    }
}
