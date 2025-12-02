using Azure;
using Moq;
using NUnit.Framework;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Repositories.ExportDetailRepo;
using SWS.Repositories.Repositories.ExportRepo;
using SWS.Repositories.Repositories.ProductRepo;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels.ExportOrderModel;
using SWS.Services.Services.ExportOrderServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWS.Test.Service
{
    [TestFixture]
    public class ExportOrderTest
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IExportOrderRepository> _mockExportOrderRepo;
        private Mock<IExportDetailRepository> _mockExportDetailRepo;
        private Mock<IProductRepository> _mockProductRepo;
        private ExportOrderService _service;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockExportOrderRepo = new Mock<IExportOrderRepository>();
            _mockExportDetailRepo = new Mock<IExportDetailRepository>();
            _mockProductRepo = new Mock<IProductRepository>();

            _mockUnitOfWork.Setup(u => u.ExportOrders).Returns(_mockExportOrderRepo.Object);
            _mockUnitOfWork.Setup(u => u.ExportDetails).Returns(_mockExportDetailRepo.Object);
            _mockUnitOfWork.Setup(u => u.Products).Returns(_mockProductRepo.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _service = new ExportOrderService(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task GetAllExportOrdersAsync_ReturnsOrders1()
        {
            //mock data
            var fakeOrders = new List<ExportOrder>
            {
                new ExportOrder { ExportOrderId = 1, Status = "Pending", InvoiceNumber = "INV001" }
            };
            _mockExportOrderRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(fakeOrders);

            var result = await _service.GetAllExportOrdersAsync();

            var responses = (result.Data as IEnumerable<ExportOrderResponse>);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, responses.Count());
            TestContext.Out.WriteLine(fakeOrders.Count);
        }

        public async Task GetAllExportOrdersAsync_ReturnsOrders2()
        {
            //mock data
            var fakeOrders = new List<ExportOrder>
            {
                new ExportOrder { ExportOrderId = 1, Status = "Pending", InvoiceNumber = "INV001" },
                new ExportOrder { ExportOrderId = 2, Status = "Completed", InvoiceNumber = "INV002" }
            };
            _mockExportOrderRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(fakeOrders);

            var result = await _service.GetAllExportOrdersAsync();

            var responses = (result.Data as IEnumerable<ExportOrderResponse>);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, responses.Count());
            TestContext.Out.WriteLine(fakeOrders.Count);
        }

        public async Task GetAllExportOrdersAsync_ReturnsOrders_null()
        {
            //mock data
            var fakeOrders = new List<ExportOrder>{};
            _mockExportOrderRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(fakeOrders);

            var result = await _service.GetAllExportOrdersAsync();

            var responses = (result.Data as IEnumerable<ExportOrderResponse>);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, responses.Count());
            TestContext.Out.WriteLine(fakeOrders.Count);
        }

        [Test]
        public async Task GetExportOrdersByIdAsync_Correct()
        {
            // Arrange
            var fakeOrders = new List<ExportOrder>
    {
        new ExportOrder { ExportOrderId = 1, Status = "Pending", InvoiceNumber = "INV001" },
        new ExportOrder { ExportOrderId = 2, Status = "Completed", InvoiceNumber = "INV002" }
    };

            _mockExportOrderRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(fakeOrders.First(f => f.ExportOrderId == 1));

            // Act
            var result = await _service.GetExportOrderByIdAsync(1);

            // Assert
            Assert.IsTrue(result.IsSuccess);

            var response = result.Data as ExportOrderResponse;
            Assert.IsNotNull(response);

            Assert.AreEqual(1, response.ExportOrderId);
            Assert.AreEqual(StatusEnums.Pending, response.Status);
            Assert.AreEqual("INV001", response.InvoiceNumber);

            TestContext.Out.WriteLine($"Returned Order ID: {response.ExportOrderId}");
        }


        [Test]
        public async Task GetExportOrdersByStatusAsync_Completed_Correct()
        {
            var fakeOrders = new List<ExportOrder>
            {
                new ExportOrder { ExportOrderId = 1, Status = "Pending", InvoiceNumber = "INV001" },
                new ExportOrder { ExportOrderId = 2, Status = "Completed", InvoiceNumber = "INV002" },
                new ExportOrder { ExportOrderId = 3, Status = "Canceled", InvoiceNumber = "INV003" },
                new ExportOrder { ExportOrderId = 4, Status = "Shipped", InvoiceNumber = "INV004" },
                new ExportOrder { ExportOrderId = 5, Status = "Canceled", InvoiceNumber = "INV005" },
            };
            _mockExportOrderRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(fakeOrders);

            var result = await _service.GetExportOrdersByStatusAsync(StatusEnums.Completed);

            //TestContext.Out.WriteLine(typeof(Assert).AssemblyQualifiedName);

            var responses = result.Data as IEnumerable<ExportOrderResponse>;
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, responses.Count());
            TestContext.Out.WriteLine(fakeOrders.Count);
        }

        [Test]
        public async Task GetExportOrdersByIdAsync_NullId()
        {
            var fakeOrders = new List<ExportOrder>
            {
                new ExportOrder { ExportOrderId = 1, Status = "Pending", InvoiceNumber = "INV001" },
                new ExportOrder { ExportOrderId = 2, Status = "Completed", InvoiceNumber = "INV002" }
            };
            _mockExportOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeOrders.First(f => f.ExportOrderId == 1));

            var result = await _service.GetExportOrderByIdAsync(1);

            //TestContext.Out.WriteLine(typeof(Assert).AssemblyQualifiedName);

            Assert.IsTrue(result.IsSuccess);

            var response = result.Data as ExportOrderResponse;
            Assert.IsNotNull(response);

            Assert.AreEqual(1, response.ExportOrderId);
            Assert.AreEqual(StatusEnums.Pending, response.Status);
            Assert.AreEqual("INV001", response.InvoiceNumber);

            TestContext.Out.WriteLine($"Returned Order ID: {response.ExportOrderId}");
        }
    }
}
