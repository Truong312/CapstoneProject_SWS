using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Repositories.UserRepo;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels.WarehouseUserModel;
using SWS.Services.Helpers;
using SWS.Services.Services.LogServices;
using SWS.Services.Services.WarehouseAuthentication;


namespace SWS.Test.Service
{
    [TestFixture]
    public class AuthenticationTest
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IUserRepository> _mockUserRepo;
        private Mock<IGoogleLoginService> _mockGoogleLogin;
        private Mock<IMapper> _mockMapper;
        private IConfiguration _configuration;
        private Mock<IActionLogService> _actionLogService;

        private WarehouseAuthenticationService _service;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockGoogleLogin = new Mock<IGoogleLoginService>();
            _mockMapper = new Mock<IMapper>();
            _actionLogService = new Mock<IActionLogService>();

            _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepo.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Mock configuration for JWT
            var inMemorySettings = new Dictionary<string, string>
                {
                    {"Jwt:Issuer", "TestIssuer"},
                    {"Jwt:Audience", "TestAudience"},
                    {"Jwt:Key", "ThisIsASecretKeyForTesting12345!"},
                    {"Jwt:AccessTokenExpirationMinutes", "60"}
                };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _service = new WarehouseAuthenticationService(
                _mockUnitOfWork.Object,
                _configuration,
                _mockGoogleLogin.Object,
                _mockMapper.Object,
                _actionLogService.Object
            );
        }

        [Test]
        public async Task RegisterAsync_NewUser_ReturnsSuccess()
        {
            var request = new RegisterWarehouseRequest
            {
                FullName = "Test User",
                Email = "test",
                Password = "Password123!",
                Phone = "0123456789",
                Address = "123 Test Street",
                Role = 1
            };

            _mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.RegisterAsync(request);

            TestContext.Out.WriteLine($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Đăng ký thành công", result.Message);
            Assert.IsNotNull(result.Data);
        }

        [Test]
        public async Task RegisterAsync_fail()
        {
            var request = new RegisterWarehouseRequest
            {
                FullName = "Test User",
                Email = "test",
                Password = "Password123!",
                Phone = "1",
                Address = "123 Test Street",
                Role = 5
            };

            _mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.RegisterAsync(request);

            TestContext.Out.WriteLine($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Đăng ký thành công", result.Message);
            Assert.IsNotNull(result.Data);
        }

        //[Test]
        //public async Task RegisterAsync_Fail()
        //{
        //    var request = new RegisterWarehouseRequest
        //    {
        //        FullName = "Test User",
        //        Email = "test",
        //        Password = "123",
        //        Phone = "0123456789",
        //        Address = "123 Test Street",
        //        Role = 1
        //    };

        //    _mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
        //    _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        //    _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        //    var result = await _service.RegisterAsync(request);

        //    TestContext.Out.WriteLine($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}");

        //    // Assert
        //    Assert.IsTrue(result.IsSuccess);
        //    Assert.AreEqual("Đăng ký thành công", result.Message);
        //    Assert.IsNotNull(result.Data);
        //}



        [Test]
        public async Task ChangePasswordAsync_OldPasswordCorrect_ReturnsSuccess()
        {
            var userId = 1;
            var oldPassword = "123";
            var newPassword = "456";
            var hashedOldPassword = PasswordHelper.HashPassword(oldPassword);

            var user = new User
            {
                UserId = userId,
                Password = hashedOldPassword
            };

            _mockUserRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockUserRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var result = await _service.ChangePasswordAsync(userId, oldPassword, newPassword);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Đổi mật khẩu thành công", result.Message);

            _mockUserRepo.Verify(r => r.UpdateAsync(It.Is<User>(u =>
                PasswordHelper.VerifyPassword(newPassword, u.Password)
            )), Times.Once);

            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ChangePasswordAsync_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = 999;
            _mockUserRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _service.ChangePasswordAsync(userId, "any", "any");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Không tìm thấy người dùng", result.Message);
        }

        [Test]
        public async Task ChangePasswordAsync_OldPasswordIncorrect_ReturnsBadRequest()
        {
            var userId = 1;
            var user = new User
            {
                UserId = userId,
                Password = PasswordHelper.HashPassword("123")
            };

            _mockUserRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            var result = await _service.ChangePasswordAsync(userId, "1234", "NewPassword");

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Mật khẩu cũ không chính xác", result.Message);
        }


        [Test]
        public async Task ChangePasswordAsync_DuplicatePassword_ReturnsBadRequest()
        {
            var userId = 1;
            var user = new User
            {
                UserId = userId,
                Password = PasswordHelper.HashPassword("abc123")
            };

            _mockUserRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            var result = await _service.ChangePasswordAsync(userId, "abc123", "abc123");

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Mật khẩu mới không được trùng với mật khẩu cũ", result.Message);
        }
    }
}
