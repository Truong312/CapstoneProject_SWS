using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SWS.BusinessObjects.Constants;
using SWS.BusinessObjects.DTOs;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Repositories.ExportRepo;
using SWS.Repositories.Repositories.ReturnRepo;
using SWS.Repositories.Repositories.UserRepo;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ReturnLookups;
using SWS.Services.ReturnOrders;
using SWS.Services.Services.WarehouseAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Test.Service
{
    [TestFixture]
    public class ReturnOrderTest
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IReturnOrderReviewService> _mockReturnOrderReviewService;
        private Mock<IReturnOrderQueryService> _mockReturnOrderQueryService;
        private Mock<IReturnOrderQueryRepository> _mockReturnOrderQueryRepository;
        private Mock<IExportOrderRepository> _mockExportOrderRepository;
        private Mock<IReturnLookupService> _mockReturnLookupService;
        private ReturnOrderReviewService _rservice;
        private ReturnOrderQueryService _qservice;
        private ReturnLookupService _lservice;
        private Mock<IReturnReasonRepository> _reasonRepoMock;
        private Mock<IReturnStatusRepository> _statusRepoMock;
        private Mock<IReturnOrderCommandRepository> _returnCmdMock;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockExportOrderRepository = new Mock<IExportOrderRepository>();
            _mockReturnOrderQueryService = new Mock<IReturnOrderQueryService>();
            _mockReturnOrderReviewService = new Mock<IReturnOrderReviewService>();
            _mockReturnLookupService = new Mock<IReturnLookupService>();
            _returnCmdMock = new Mock<IReturnOrderCommandRepository>();
            //_rservice = new Mock<ReturnOrderReviewService>();
            _reasonRepoMock = new Mock<IReturnReasonRepository>();
            _statusRepoMock = new Mock<IReturnStatusRepository>();
            _mockReturnOrderQueryRepository = new Mock<IReturnOrderQueryRepository>();

            _mockUnitOfWork.Setup(u => u.ReturnOrdersQuery)
                    .Returns(_mockReturnOrderQueryRepository.Object);

            _mockUnitOfWork.Setup(u => u.ReturnReasons).Returns(_reasonRepoMock.Object);
            _mockUnitOfWork.Setup(u => u.ReturnStatuses).Returns(_statusRepoMock.Object);
            _mockUnitOfWork.Setup(u => u.ReturnOrdersCommand).Returns(_returnCmdMock.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);


            _rservice = new ReturnOrderReviewService(
                _mockUnitOfWork.Object
            );

            _qservice = new ReturnOrderQueryService(
                _mockUnitOfWork.Object
            );


        }

        [Test]
        public async Task GetReasonsAsync_ShouldReturnMappedDtos()
        {
            var query = "damaged";

            var reasonsData = new List<ReturnReason>
            {
                new ReturnReason { ReasonId = 1, ReasonCode = "DAM", Description = "Damaged item" },
                new ReturnReason { ReasonId = 2, ReasonCode = "EXP", Description = "Expired product" }
            };

            _reasonRepoMock.Setup(r => r.SearchAsync(query))
                .ReturnsAsync(reasonsData);

            var result = await _lservice.GetReasonsAsync(query);


            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].ReasonId, Is.EqualTo(1));
            Assert.That(result[0].ReasonCode, Is.EqualTo("DAM"));
            Assert.That(result[0].Description, Is.EqualTo("Damaged item"));

            _reasonRepoMock.Verify(r => r.SearchAsync(query), Times.Once);
        }


        [Test]
        public async Task GetStatusesAsync_ShouldReturnRepositoryResult()
        {
            var query = "pending";

            var statusDtos = new List<ReturnStatusDto>
            {
                new ReturnStatusDto("PENDING", 1),
                new ReturnStatusDto("COMPLETED", 2)
            };

            _statusRepoMock.Setup(s => s.SearchAsync(query))
                .ReturnsAsync(statusDtos);

            var result = await _lservice.GetStatusesAsync(query);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Status, Is.EqualTo(1));
            Assert.That(result[0].Status, Is.EqualTo("PENDING"));

            _statusRepoMock.Verify(s => s.SearchAsync(query), Times.Once);
        }

        [Test]
        public async Task ReviewAsync_Approve_ReturnsSuccess()
        {
            var order = new ReturnOrder
            {
                ReturnOrderId = 10,
                Status = ReturnStatuses.Pending,
                ReturnOrderDetails = new List<ReturnOrderDetail>()
            };

            _returnCmdMock
                .Setup(r => r.GetForUpdateAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var req = new ReviewReturnOrderRequest
            {
                ReturnOrderId = 10,
                Decision = "approve"
            };

            var result = await _rservice.ReviewAsync(99, req);

            Assert.That(result.ReturnOrderId, Is.EqualTo(10));
            Assert.That(result.Status, Is.EqualTo(ReturnStatuses.Approved));
            Assert.That(order.ReviewedBy, Is.EqualTo(99));

            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
            _returnCmdMock.Verify(r =>
                r.AddActionLogAsync(It.IsAny<ActionLog>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Test]
        public async Task ReviewAsync_Reject_ReturnsRejected()
        {
            var order = new ReturnOrder
            {
                ReturnOrderId = 20,
                Status = ReturnStatuses.Pending,
                ReturnOrderDetails = new List<ReturnOrderDetail>()
            };

            _returnCmdMock
                .Setup(r => r.GetForUpdateAsync(20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var req = new ReviewReturnOrderRequest
            {
                ReturnOrderId = 20,
                Decision = "reject"
            };

            var result = await _rservice.ReviewAsync(100, req);

            Assert.That(result.Status, Is.EqualTo(ReturnStatuses.Rejected));
        }


        [Test]
        public void ReviewAsync_NotFound()
        {
            _returnCmdMock
                .Setup(r => r.GetForUpdateAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ReturnOrder?)null);

            var req = new ReviewReturnOrderRequest { ReturnOrderId = 1 };

            Assert.ThrowsAsync<KeyNotFoundException>(
                () => _rservice.ReviewAsync(1, req));
        }


        [Test]
        public void ReviewAsync_InvalidStatus()
        {
            var order = new ReturnOrder
            {
                ReturnOrderId = 1,
                Status = ReturnStatuses.Approved
            };

            _returnCmdMock
                .Setup(r => r.GetForUpdateAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var req = new ReviewReturnOrderRequest { ReturnOrderId = 1 };

            Assert.ThrowsAsync<InvalidOperationException>(
                () => _rservice.ReviewAsync(10, req));
        }


        [Test]
        public async Task ReviewAsync_UpdatesLineItems()
        {
            var order = new ReturnOrder
            {
                ReturnOrderId = 1,
                Status = ReturnStatuses.Pending,
                ReturnOrderDetails = new List<ReturnOrderDetail>
                {
                    new ReturnOrderDetail{ ReturnDetailId = 10, ActionId = null, Note = "" }
                }
            };

            _returnCmdMock
                .Setup(r => r.GetForUpdateAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var req = new ReviewReturnOrderRequest
            {
                ReturnOrderId = 1,
                Lines = new List<ReviewReturnOrderLineDto>
                {
                    new ReviewReturnOrderLineDto
                    {
                        ReturnDetailId = 10,
                        ActionId = 5,
                        Note = "Fix item"
                    }
                }
            };

            await _rservice.ReviewAsync(55, req);

            var detail = order.ReturnOrderDetails.First();
            Assert.That(detail.ActionId, Is.EqualTo(5));
            Assert.That(detail.Note, Is.EqualTo("Fix item"));
        }


        [Test]
        public async Task ReviewAsync_AppendsNoteCorrectly()
        {
            var order = new ReturnOrder
            {
                ReturnOrderId = 1,
                Status = ReturnStatuses.Pending,
                Note = "Existing note",
                ReturnOrderDetails = new List<ReturnOrderDetail>()
            };

            _returnCmdMock
                .Setup(r => r.GetForUpdateAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var req = new ReviewReturnOrderRequest
            {
                ReturnOrderId = 1,
                Note = "New review note"
            };

            await _rservice.ReviewAsync(5, req);

            Assert.That(
                order.Note,
                Is.EqualTo("Existing note\n[Review] New review note")
            );
        }

        [Test]
        public async Task GetListAsync_ReturnsMappedList()
        {
            var data = new List<ReturnOrder>
            {
                new ReturnOrder
                {
                    ReturnOrderId = 1,
                    ExportOrderId = 100,
                    CheckInTime = DateTime.UtcNow,
                    Status = "Pending",
                    Note = "Test",
                    CheckedByNavigation = new User { FullName = "Checker A" },
                    ReviewedByNavigation = new User { FullName = "Reviewer B" }
                }
            };

            _mockReturnOrderQueryRepository
                .Setup(r => r.GetListAsync(null, null, null, null, null, null))
                .ReturnsAsync(data);

            var result = await _qservice.GetListAsync(null, null, null, null, null, null);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ReturnOrderId, Is.EqualTo(1));
            Assert.That(result[0].CheckedByName, Is.EqualTo("Checker A"));
            Assert.That(result[0].ReviewedByName, Is.EqualTo("Reviewer B"));
        }


        [Test]
        public async Task GetDetailAsync_WhenNotFound_ReturnsNull()
        {
            _mockReturnOrderQueryRepository
                .Setup(r => r.GetDetailAsync(999))
                .ReturnsAsync((ReturnOrder?)null);

            var result = await _qservice.GetDetailAsync(999);

            Assert.That(result, Is.Null);
        }


        [Test]
        public async Task GetDetailAsync_ReturnsMappedDetail()
        {
            var order = new ReturnOrder
            {
                ReturnOrderId = 1,
                ExportOrderId = 200,
                CheckInTime = DateTime.UtcNow,
                Status = "Pending",
                Note = "Header note",
                CheckedByNavigation = new User { FullName = "Checker A" },
                ReviewedByNavigation = new User { FullName = "Reviewer B" },
                ReturnOrderDetails = new List<ReturnOrderDetail>
                {
                    new ReturnOrderDetail
                    {
                        ReturnDetailId = 10,
                        ProductId = 99,
                        Quantity = 5,
                        ReasonId = 3,
                        Note = "Line note",
                        ActionId = 2,
                        LocationId = 7,
                        Product = new Product { Name = "Laptop" },
                        Reason = new ReturnReason { ReasonCode = "DAMAGE" }
                    }
                }
            };

            _mockReturnOrderQueryRepository
                .Setup(r => r.GetDetailAsync(1))
                .ReturnsAsync(order);

            var result = await _qservice.GetDetailAsync(1);

            Assert.That(result, Is.Not.Null);

            // HEADER
            Assert.That(result!.Header.ReturnOrderId, Is.EqualTo(1));
            Assert.That(result.Header.CheckedByName, Is.EqualTo("Checker A"));
            Assert.That(result.Header.ReviewedByName, Is.EqualTo("Reviewer B"));

            // LINE
            var line = result.Lines.First();
            Assert.That(line.ReturnDetailId, Is.EqualTo(10));
            Assert.That(line.ProductName, Is.EqualTo("Laptop"));
            Assert.That(line.ReasonCode, Is.EqualTo("DAMAGE"));
            Assert.That(line.Quantity, Is.EqualTo(5));
            Assert.That(line.Note, Is.EqualTo("Line note"));
            Assert.That(line.ActionId, Is.EqualTo(2));
            Assert.That(line.LocationId, Is.EqualTo(7));
        }
    }
}
