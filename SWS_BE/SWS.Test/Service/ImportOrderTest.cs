using Moq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SWS.BusinessObjects.DTOs;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Repositories.ImportOrders;
using SWS.Repositories.Repositories.ProductRepo;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels;
using SWS.Services.ApiModels.ExportOrderModel;
using SWS.Services.ApiModels.ImportOrders;
using SWS.Services.ImportOrders;
using SWS.Services.Services.ExportOrderServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWS.Test.Service
{
    [TestFixture]
    public class ImportOrderTest
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IImportOrderQueryRepository> _mockImportQueryRepo;
        private Mock<IImportOrderCommandRepository> _mockImportCommandRepo;
        private Mock<IProductRepository> _mockProductRepo;
        private ImportOrderQueryService _qservice;
        private ImportOrderCommandService _cservice;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockImportQueryRepo = new Mock<IImportOrderQueryRepository>();
            _mockImportCommandRepo = new Mock<IImportOrderCommandRepository>();
            _mockProductRepo = new Mock<IProductRepository>();

            _mockUnitOfWork.Setup(u => u.ImportOrdersQuery).Returns(_mockImportQueryRepo.Object);
            _mockUnitOfWork.Setup(u => u.ImportOrdersCommand).Returns(_mockImportCommandRepo.Object);
            _mockUnitOfWork.Setup(u => u.Products).Returns(_mockProductRepo.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _cservice = new ImportOrderCommandService(_mockUnitOfWork.Object);
            _qservice = new ImportOrderQueryService(_mockUnitOfWork.Object);
        }


        [Test]
        public async Task GetAllImportOrdersAsync_ReturnsOrders1()
        {
            // Arrange
            var fakeOrders = new List<ImportOrderListItemDto>
    {
        new ImportOrderListItemDto(
            1,
            "INV-2025-001",
            new DateOnly(2025, 03, 15),
            "ABC Supplier Co., Ltd",
            "Pending",
            12,
            "Nguyen Van A"
        )
    };

            var fakeResult = new ImportOrderListResult(
                Total: fakeOrders.Count,
                Page: 1,
                PageSize: 10,
                Items: fakeOrders
            );

            _mockImportQueryRepo
                .Setup(r => r.GetListAsync(
                    It.IsAny<ImportOrderListQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeResult);

            var serviceResult = await _qservice.GetListAsync(new ImportOrderListQuery());

            var responses = serviceResult.Items;

            // Assert
            Assert.IsNotNull(serviceResult);
            Assert.AreEqual(1, serviceResult.Total);
            Assert.AreEqual(1, serviceResult.Items.Count());
        }





        [Test]
        public async Task CreateAsync_Correct1()
        {
            //mock data
            var request = new CreateImportOrderRequest
            {
                ProviderId = 5,
                OrderDate = new DateOnly(2025, 3, 10),
                InvoiceNumber = "MOCK",
                Items = new List<CreateImportOrderItem>
                {
                    new CreateImportOrderItem
                    {
                        ProductId = 101,
                        Quantity = 25,
                        ImportPrice = 150000m
                    }
                }
            };

            var fakeResponse = new CreateImportOrderResponse(1, "MOCK");

            _mockUnitOfWork.Setup(u => u.ImportOrdersCommand.CreateAsync(1, request, It.IsAny<CancellationToken>()))
    .ReturnsAsync(fakeResponse);

            var result = await _cservice.CreateAsync(1, request);

            Assert.IsNotNull(result);
            Assert.That(result.ImportOrderId, Is.GreaterThan(0));
            Assert.IsNotNull(result.InvoiceNumber);

            TestContext.Out.WriteLine($"New Order ID = {result.ImportOrderId}");
        }


        [Test]
        public async Task ReviewAsync_Approve_ReturnsCompletedMessage()
        {
            // Arrange
            int importOrderId = 1;
            int reviewerId = 10;
            var req = new ReviewImportOrderRequest
            {
                Approve = true,
                Note = "Everything OK"
            };

            _mockImportCommandRepo
                .Setup(r => r.ReviewAsync(importOrderId, reviewerId, true, "Everything OK", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _cservice.ReviewAsync(importOrderId, reviewerId, req);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Duyệt phiếu nhập (COMPLETED) và cập nhật tồn kho thành công.", result.Message);
            Assert.IsTrue(result.Data);
        }



        [Test]
        public async Task ReviewAsync_Reject_ReturnsCanceledMessage()
        {
            // Arrange
            int importOrderId = 2;
            int reviewerId = 11;
            var req = new ReviewImportOrderRequest
            {
                Approve = false,
                Note = "Invalid documents"
            };

            _mockImportCommandRepo
                .Setup(r => r.ReviewAsync(importOrderId, reviewerId, false, "Invalid documents", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _cservice.ReviewAsync(importOrderId, reviewerId, req);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Đã hủy phiếu nhập (CANCELED).", result.Message);
            Assert.IsTrue(result.Data);
        }



        [Test]
        public async Task ReviewAsync_Exception_ReturnsError()
        {
            // Arrange
            int importOrderId = 3;
            int reviewerId = 12;
            var req = new ReviewImportOrderRequest
            {
                Approve = true,
                Note = "Error test"
            };

            _mockImportCommandRepo
                .Setup(r => r.ReviewAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB Error"));

            // Act
            var result = await _cservice.ReviewAsync(importOrderId, reviewerId, req);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("DB Error", result.Message);
            Assert.IsFalse(result.Data);
        }

    }
}
