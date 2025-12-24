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
                SerialNumber = "SN001",
                Name = "Test Product",
                ExpiredDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(6)),
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
        public async Task AddProductAsync_Error()
        {
            // Arrange
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


    }
}
