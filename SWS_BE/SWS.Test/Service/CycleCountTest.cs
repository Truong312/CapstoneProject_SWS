using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Repositories.CycleCountDetailRepo;
using SWS.Repositories.Repositories.CycleCountRepo;
using SWS.Repositories.Repositories.InventoryRepo;
using SWS.Repositories.Repositories.ProductRepo;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels.Commons;
using SWS.Services.Services.CycleCountServices;
using SWS.Services.Services.LogServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWS.Tests.Services
{
    [TestFixture]
    public class CycleCountServiceTests
    {
        private Mock<IUnitOfWork> _uow;
        private Mock<ICycleCountRepository> _cycleRepo;
        private Mock<ICycleCountDetailRepository> _detailRepo;
        private Mock<IProductRepository> _productRepo;
        private Mock<IInventoryRepository> _inventoryRepo;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<IActionLogService> _actionLogService;
        private CycleCountService _service;

        [SetUp]
        public void Setup()
        {
            _uow = new Mock<IUnitOfWork>();
            _cycleRepo = new Mock<ICycleCountRepository>();
            _detailRepo = new Mock<ICycleCountDetailRepository>();
            _productRepo = new Mock<IProductRepository>();
            _inventoryRepo = new Mock<IInventoryRepository>();
            _actionLogService = new Mock<IActionLogService>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _uow.Setup(u => u.CycleCounts).Returns(_cycleRepo.Object);
            _uow.Setup(u => u.CycleCountDetails).Returns(_detailRepo.Object);
            _uow.Setup(u => u.Products).Returns(_productRepo.Object);
            _uow.Setup(u => u.Inventories).Returns(_inventoryRepo.Object);

            _service = new CycleCountService(_uow.Object, _httpContextAccessor.Object, _actionLogService.Object);
        }


        public async Task StartCycleCountAsync_CreatesCycle_AndDetails()
        {
            int userId = 10;

            _productRepo.Setup(p => p.GetAllAsync())
                        .ReturnsAsync(new List<Product>
                        {
                            new Product { ProductId = 1 },
                            new Product { ProductId = 2 }
                        });

            _productRepo.Setup(p => p.GetProductQuantity(It.IsAny<int>()))
                        .ReturnsAsync(50);

            _cycleRepo.Setup(r => r.AddAsync(It.IsAny<CycleCount>()))
                      .Callback<CycleCount>(c => c.CycleCountId = 123)
                      .Returns(Task.CompletedTask);

            var result = await _service.StartCycleCountAsync();

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));

            _cycleRepo.Verify(r => r.AddAsync(It.IsAny<CycleCount>()), Times.Once);
            _detailRepo.Verify(r => r.AddAsync(It.IsAny<CycleCountDetail>()), Times.Exactly(2));
            _uow.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
        }


        [Test]
        public async Task UpdateCountedQuantityAsync_WhenDetailFound_UpdatesQuantity()
        {
            var detail = new CycleCountDetail { DetailId = 10 };

            _detailRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(detail);

            var result = await _service.UpdateCountedQuantityAsync(detail.DetailId, 1, 99);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(detail.CountedQuantity, Is.EqualTo(99));

            _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateCountedQuantityAsync_NotFound()
        {
            _detailRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((CycleCountDetail?)null);

            var result = await _service.UpdateCountedQuantityAsync(999, 1, 20);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Does.Contain("Không tìm thấy cycle count detail"));
        }


        [Test]
        public async Task FinalizeCycleCountAsync_CycleNotFound()
        {
            _cycleRepo.Setup(r => r.GetByIdAsync(123)).ReturnsAsync((CycleCount?)null);

            var result = await _service.FinalizeCycleCountAsync(123);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task FinalizeCycleCountAsync_UpdatesInventory()
        {
            var cycle = new CycleCount { CycleCountId = 5, Status = "Pending" };

            _cycleRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(cycle);

            var details = new List<CycleCountDetail>
            {
                new CycleCountDetail { ProductId = 1, CountedQuantity = 100 }
            };

            _detailRepo.Setup(r => r.GetAllByCycleCountId(5)).ReturnsAsync(details);
            _inventoryRepo.Setup(r => r.GetByProductId(1))
                          .ReturnsAsync(new Inventory { QuantityAvailable = 80 });
            _productRepo.Setup(r => r.GetByIdAsync(1))
                        .ReturnsAsync(new Product { ProductId = 1, SerialNumber = "ABC001" });

            var result = await _service.FinalizeCycleCountAsync(5);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(cycle.Status, Is.EqualTo(StatusEnums.Completed.ToString()));

            _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task FinalizeCycleCountAsync_ProductNotFound()
        {
            var cycle = new CycleCount { CycleCountId = 5 };

            _cycleRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(cycle);

            _detailRepo.Setup(r => r.GetAllByCycleCountId(5))
                       .ReturnsAsync(new List<CycleCountDetail>
                       {
                           new CycleCountDetail { ProductId = 2, CountedQuantity = 80 }
                       });

            _inventoryRepo.Setup(r => r.GetByProductId(2))
                          .ReturnsAsync(new Inventory { QuantityAvailable = 50 });

            _productRepo.Setup(r => r.GetByIdAsync(2))
                        .ReturnsAsync((Product?)null);

            var result = await _service.FinalizeCycleCountAsync(5);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Does.Contain("Lỗi xảy ra khi tìm product"));
        }
    }
}
