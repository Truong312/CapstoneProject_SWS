using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SWS.Repositories.Repositories.ExportRepo;
using SWS.Repositories.Repositories.UserRepo;
using SWS.Repositories.UnitOfWork;
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
        private Mock<IExportOrderRepository> _mockExportOrderRepository;
        private ReturnOrderReviewService _service;

        //[SetUp]
        //public void Setup()
        //{
        //    _mockUnitOfWork = new Mock<IUnitOfWork>();
        //    _mockUserRepo = new Mock<IUserRepository>();

        //    _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);


        //    _service = new ReturnOrderReviewService(
        //        _mockUnitOfWork.Object,
        //        _configuration,
        //        _mockGoogleLogin.Object,
        //        _mockMapper.Object
        //    );
        //}
    }
}
