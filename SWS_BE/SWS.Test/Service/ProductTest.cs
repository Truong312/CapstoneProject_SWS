using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Repositories.ExportDetailRepo;
using SWS.Repositories.Repositories.ExportRepo;
using SWS.Repositories.Repositories.ProductRepo;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.ProductModel;
using SWS.Services.Services.ExportOrderServices;
using SWS.Services.Services.LogServices;
using SWS.Services.Services.ProductServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Test.Service
{
    [TestFixture]
    public class ProductTest
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IExportOrderRepository> _mockExportOrderRepo;
        private Mock<IExportDetailRepository> _mockExportDetailRepo;
        private Mock<IProductRepository> _mockProductRepo;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<IActionLogService> _actionLogService;
        private Mock<ITransactionLogService> _transactionLogService;
        private WarehouseProductService _service;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            //_mockExportOrderRepo = new Mock<IExportOrderRepository>();
            //_mockExportDetailRepo = new Mock<IExportDetailRepository>();
            _mockProductRepo = new Mock<IProductRepository>();
            _actionLogService = new Mock<IActionLogService>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _transactionLogService = new Mock<ITransactionLogService>();

            _mockUnitOfWork.Setup(u => u.Products).Returns(_mockProductRepo.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _service = new WarehouseProductService(_mockUnitOfWork.Object, _httpContextAccessor.Object, _actionLogService.Object);
        }

        [Test]
        public async Task GetProductByIdAsync_ProductExists_ReturnsProduct()
        {
            var productId = 1;
            var fakeProduct = new Product
            {
                ProductId = productId,
                SerialNumber = "SN001",
                Name = "Product A",
                ExpiredDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(6)),
                Unit = "Box",
                UnitPrice = 100000,
                ReceivedDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)),
                PurchasedPrice = 90000,
                ReorderPoint = 5,
                Image = "image.png",
                Description = "Test product"
            };

            _mockProductRepo
                .Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync(fakeProduct);

            var result = await _service.GetProductByIdAsync(productId);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode); //success
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(productId, result.Data.ProductId);
            Assert.AreEqual("Product A", result.Data.Name);
        }

        [Test]
        public async Task GetProductByIdAsync_ProductNotFound()
        {
            var productId = 999;

            _mockProductRepo
                .Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync((Product?)null);

            var result = await _service.GetProductByIdAsync(productId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode); //return 404
            Assert.AreEqual("Không tìm thấy sản phẩm", result.Message);
            Assert.IsNull(result.Data);
        }


        [Test]
        public async Task GetProductByIdAsync_Error()
        {
            var productId = 1;

            _mockProductRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _service.GetProductByIdAsync(productId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode); //return 500
            StringAssert.Contains("DB error", result.Message);
        }



        [Test]
        public async Task AddProductAsync_Success()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                SerialNumber = "",
                Name = "Test Product",
                ExpiredDate = DateOnly.FromDateTime(new DateTime(2025, 6, 4)),
                Unit = "Box",
                UnitPrice = 100,
                ReceivedDate = DateOnly.FromDateTime(DateTime.Now),
                PurchasedPrice = 80,
                ReorderPoint = 10,
                Image = "image.png",
                Description = "Test description"
            };

            _mockUnitOfWork
                .Setup(u => u.Products.AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            _actionLogService
                .Setup(a => a.CreateActionLogAsync(
                    ActionType.Create,
                    "Product",
                    "Thêm sản phẩm mới"))
                .ReturnsAsync(new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status201Created
                });

            var result = await _service.AddProductAsync(request);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
            Assert.AreEqual("Thêm sản phẩm thành công", result.Message);

            _mockUnitOfWork.Verify(u => u.Products.AddAsync(It.IsAny<Product>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            _actionLogService.Verify(
                a => a.CreateActionLogAsync(ActionType.Create, "Product", "Thêm sản phẩm mới"),
                Times.Once);
        }

        [Test]
        public async Task AddProductAsync_eXPIRED()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                SerialNumber = "",
                Name = "Test Product",
                ExpiredDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                Unit = "Box",
                UnitPrice = 100,
                ReceivedDate = DateOnly.FromDateTime(DateTime.Now),
                PurchasedPrice = 80,
                ReorderPoint = 10,
                Image = "image.png",
                Description = "Test description"
            };

            _mockUnitOfWork
                .Setup(u => u.Products.AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            _actionLogService
                .Setup(a => a.CreateActionLogAsync(
                    ActionType.Create,
                    "Product",
                    "Thêm sản phẩm mới"))
                .ReturnsAsync(new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status201Created
                });

            var result = await _service.AddProductAsync(request);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
            Assert.AreEqual("Thêm sản phẩm thành công", result.Message);

            _mockUnitOfWork.Verify(u => u.Products.AddAsync(It.IsAny<Product>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            _actionLogService.Verify(
                a => a.CreateActionLogAsync(ActionType.Create, "Product", "Thêm sản phẩm mới"),
                Times.Once);
        }

        [Test]
        public async Task AddProductAsync_WhenExceptionThrown_ReturnsError()
        {

            var request = new CreateProductRequest
            {
                SerialNumber = "SN-ERROR",
                Name = "Error Product"
            };

            _mockProductRepo
                .Setup(r => r.AddAsync(It.IsAny<Product>()))
                .ThrowsAsync(new Exception("DB Error"));

            // Act
            var result = await _service.AddProductAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.IsTrue(result.Message.Contains("DB Error"));

            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        private CreateProductRequest ValidRequest() => new CreateProductRequest
        {
            SerialNumber = "SN001",
            Name = "Test Product",
            ExpiredDate = new DateOnly(2026, 1, 1),
            Unit = "Box",
            UnitPrice = 100,
            ReceivedDate = DateOnly.FromDateTime(DateTime.Today),
            PurchasedPrice = 80,
            ReorderPoint = 10,
            Image = "image.png",
            Description = "Test"
        };

        [Test]
        public async Task AddProductAsync_NegativePrices_StillReturnsCreated()
        {
            var request = new CreateProductRequest
            {
                SerialNumber = "SN-NEG",
                Name = "Negative Price Product",
                UnitPrice = -100,
                PurchasedPrice = -50,
                ReorderPoint = -50,
            };

            var result = await _service.AddProductAsync(request);

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async Task AddProductAsync_EmptyStrings_StillReturnsCreated()
        {
            var request = new CreateProductRequest
            {
                SerialNumber = "",
                Name = "",
                Unit = "",
                Image = "",
                Description = ""
            };

            var result = await _service.AddProductAsync(request);

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async Task DeleteProductAsync_WhenProductExists_ReturnsSuccess()
        {
            var product = new Product { ProductId = 1 };

            _mockProductRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            var result = await _service.DeleteProductAsync(1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            _mockProductRepo.Verify(r => r.DeleteAsync(product), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }


        [Test]
        public async Task DeleteProductAsync_WhenProductNotFound_Returns404()
        {
            _mockProductRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Product)null);

            var result = await _service.DeleteProductAsync(99);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);

            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task GetNearExpiredProductsAsync_ReturnsProducts()
        {
            var products = new List<Product>
    {
        new Product { ProductId = 1, Name = "Milk" }
    };

            _mockProductRepo
                .Setup(r => r.GetNearExpiredProductsAsync(It.IsAny<DateOnly>()))
                .ReturnsAsync(products);

            var result = await _service.GetNearExpiredProductsAsync();

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Data.Count());
        }

        [Test]
        public async Task GetNearExpiredProductsAsync_WhenEmpty_ReturnsEmptyList()
        {
            _mockProductRepo
                .Setup(r => r.GetNearExpiredProductsAsync(It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<Product>());

            var result = await _service.GetNearExpiredProductsAsync();

            Assert.IsTrue(result.IsSuccess);
            Assert.IsEmpty(result.Data);
        }

        [Test]
        public async Task GetExpiredProductsAsync_ReturnsSuccess()
        {
            _mockProductRepo
                .Setup(r => r.GetExpiredProductsAsync(It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<Product> { new Product() });

            var result = await _service.GetExpiredProductsAsync();

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Data.Count());
        }

        [Test]
        public async Task GetExpiredProductsAsync_WhenException_Returns500()
        {
            _mockProductRepo
                .Setup(r => r.GetExpiredProductsAsync(It.IsAny<DateOnly>()))
                .ThrowsAsync(new Exception("Expired error"));

            var result = await _service.GetExpiredProductsAsync();

            Assert.IsFalse(result.IsSuccess);
        }


        [Test]
        public async Task GetLowStockProductsAsync_ReturnsSuccess()
        {
            _mockProductRepo
                .Setup(r => r.GetLowStockProductsAsync())
                .ReturnsAsync(new List<Product> { new Product() });

            var result = await _service.GetLowStockProductsAsync();

            Assert.IsTrue(result.IsSuccess);
        }


        [Test]
        public async Task GetLowStockProductsAsync_WhenException_Returns500()
        {
            _mockProductRepo
                .Setup(r => r.GetLowStockProductsAsync())
                .ThrowsAsync(new Exception("Stock error"));

            var result = await _service.GetLowStockProductsAsync();

            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task SearchProductsAsync_WithKeyword_ReturnsResults()
        {
            _mockProductRepo
                .Setup(r => r.SearchAsync("milk"))
                .ReturnsAsync(new List<Product> { new Product { Name = "Milk" } });

            var result = await _service.SearchProductsAsync("milk");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Data.Count());
        }

        public async Task SearchProductsAsync_WithKeyword_Fail()
        {
            _mockProductRepo
                .Setup(r => r.SearchAsync("21233"))
                .ReturnsAsync(new List<Product> { new Product { Name = "21233" } });

            var result = await _service.SearchProductsAsync("21233");

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Test]
        public async Task SearchProductsAsync_WithEmptyText_ReturnsResults()
        {
            _mockProductRepo
                .Setup(r => r.SearchAsync(""))
                .ReturnsAsync(new List<Product>());

            var result = await _service.SearchProductsAsync("");

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async Task SearchProductsAsync_WhenException_Returns500()
        {
            _mockProductRepo
                .Setup(r => r.SearchAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Search error"));

            var result = await _service.SearchProductsAsync("abc");

            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task GetProductsPagedAsync_WhenRequestNull_UsesDefaults()
        {
            _mockProductRepo
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Product>
                {
            new Product { ProductId = 1 },
            new Product { ProductId = 2 }
                });

            var result = await _service.GetProductsPagedAsync(null);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Data.Total);
            Assert.AreEqual(1, result.Data.Page);
            Assert.AreEqual(20, result.Data.PageSize);
        }

        [Test]
        public async Task GetProductsPagedAsync_WithSearch_UsesSearchRepo()
        {
            var req = new PagedRequestDto { Q = "milk", Page = 1, PageSize = 10 };

            _mockProductRepo
                .Setup(r => r.SearchAsync("milk"))
                .ReturnsAsync(new List<Product> { new Product() });

            var result = await _service.GetProductsPagedAsync(req);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Data.Items.Count());

        }


        private Product ExistingProduct() => new Product
        {
            ProductId = 1,
            Name = "Old Name",
            Unit = "Box",
            UnitPrice = 100,
            PurchasedPrice = 80,
            ReorderPoint = 10,
            ExpiredDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30))
        };

        [Test]
        public async Task UpdateProductAsync_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var product = ExistingProduct();

            _mockProductRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(product);

            var request = new UpdateProductRequest
            {
                Name = "Updated Name",
                UnitPrice = 120
            };

            // Act
            var result = await _service.UpdateProductAsync(1, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
                Assert.AreEqual("Updated Name", product.Name);
                Assert.AreEqual(120, product.UnitPrice);
            });

            _mockProductRepo.Verify(r => r.UpdateAsync(product), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync());
        }


        [Test]
        public async Task UpdateProductAsync_ProductNotFound_Returns404()
        {
            // Arrange
            _mockProductRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Product)null);

            var request = new UpdateProductRequest
            {
                Name = "New Name"
            };

            // Act
            var result = await _service.UpdateProductAsync(1, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
                Assert.IsTrue(result.Message.Contains("Không tìm thấy"));
            });

            _mockProductRepo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task UpdateProductAsync_EmptyRequest_ReturnsSuccess()
        {
            // Arrange
            var product = ExistingProduct();

            _mockProductRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(product);

            var request = new UpdateProductRequest(); // all null

            // Act
            var result = await _service.UpdateProductAsync(1, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            });

            _mockProductRepo.Verify(r => r.UpdateAsync(product), Times.Once);
        }

        [Test]
        public async Task UpdateProductAsync_UpdateDates_ReturnsSuccess()
        {
            // Arrange
            var product = ExistingProduct();
            var newDate = DateOnly.FromDateTime(DateTime.Now.AddDays(90));

            _mockProductRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(product);

            var request = new UpdateProductRequest
            {
                ExpiredDate = newDate
            };

            // Act
            var result = await _service.UpdateProductAsync(1, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(newDate, product.ExpiredDate);
            });
        }



    }
}





    

