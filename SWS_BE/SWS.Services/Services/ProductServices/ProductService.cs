using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Dtos.Product;
using SWS.BusinessObjects.Models;
using SWS.Repositories.UnitOfWork;

namespace SWS.Services.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private bool ValidateProduct(Product product)
        {
            if (product.ExpiredDate <= product.ReceivedDate || product.UnitPrice < product.PurchasedPrice)
            {
                return false;
            }
            return true;
        }
        public async Task<Product?> CreateProductAsync(CreateProductDto createProductDto)
        {
            try
            {
                var product = new Product
                {
                    SerialNumber = createProductDto.SerialNumber,
                    Name = createProductDto.Name,
                    ExpiredDate = createProductDto.ExpiredDate,
                    Unit = createProductDto.Unit,
                    UnitPrice = createProductDto.UnitPrice,
                    ReceivedDate = createProductDto.ReceivedDate,
                    PurchasedPrice = createProductDto.PurchasedPrice,
                    ReorderPoint = createProductDto.ReorderPoint,
                    Image = createProductDto.Image,
                    Description = createProductDto.Description
                };
                if (!ValidateProduct(product))
                {
                    throw new Exception("ExpiredDate should be later than ReceivedDate.");
                }
                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
                return product;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Product?> UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(updateProductDto.ProductId);
                if (product == null)
                {
                    return null;
                }

                product.ProductId = updateProductDto.ProductId;
                product.SerialNumber = updateProductDto.SerialNumber;
                product.Name = updateProductDto.Name;
                product.ExpiredDate = updateProductDto.ExpiredDate;
                product.Unit = updateProductDto.Unit;
                product.UnitPrice = updateProductDto.UnitPrice;
                product.ReceivedDate = updateProductDto.ReceivedDate;
                product.PurchasedPrice = updateProductDto.PurchasedPrice;
                product.ReorderPoint = updateProductDto.ReorderPoint;
                product.Image = updateProductDto.Image;
                product.Description = updateProductDto.Description;


                //Checking before update
                if (!ValidateProduct(product))
                {
                    throw new Exception("ExpiredDate should be later than ReceivedDate.");
                }

                await _unitOfWork.SaveChangesAsync();
                return product;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
