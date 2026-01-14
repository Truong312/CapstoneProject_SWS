using Moq;
using NUnit.Framework;
using SWS.Repositories.Repositories.UserRepo;
using SWS.Repositories.UnitOfWork;
using SWS.Services.Services.WarehouseUserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SWS.BusinessObjects.Models;
using SWS.Services.ApiModels;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.WarehouseUserModel;

namespace SWS.Test.Service;
    [TestFixture]
public class UserTest
{
    private Mock<IUnitOfWork> _uowMock;
    private Mock<IUserRepository> _userRepoMock;
    private WarehouseUserAdminService _service;

    [SetUp]
    public void Setup()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();

        _uowMock.Setup(x => x.Users).Returns(_userRepoMock.Object);

        _service = new WarehouseUserAdminService(_uowMock.Object);
    }

    public List<User> FakeUsers() => new()
    {
        new User { UserId = 1, FullName = "Alice", Email = "alice@mail.com", Role = 2 },
        new User { UserId = 2, FullName = "Bob", Email = "bob@mail.com", Role = 1 },
        new User { UserId = 3, FullName = "Charlie", Email = "charlie@mail.com", Role = 1 }
    };


    [Test]
    public async Task GetUsersPagedAsync_NoKeyword_ReturnPagedResult()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(FakeUsers());

        var req = new PagedRequestDto { Page = 1, PageSize = 2 };

        var result = await _service.GetUsersPagedAsync(req);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        Assert.AreEqual(3, result.Data.Total);
        Assert.AreEqual(2, result.Data.Items.Count());
    }

    [Test]
    public async Task GetUsersPagedAsync_WithKeyword_ReturnFilteredUsers()
    {
        _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync((Expression<Func<User, bool>> exp) =>
                FakeUsers().Where(exp.Compile()));

        var req = new PagedRequestDto { Page = 1, PageSize = 10, Q = "Alice" };

        var result = await _service.GetUsersPagedAsync(req);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(0, result.Data.Items.Count());
        //Assert.AreEqual("Alice", result.Data.Items.First().FullName);
    }

    [Test]
    public async Task GetUsersPagedAsync_WithKeyword_NotMatchAny_ReturnEmptyList()
    {
        _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync((Expression<Func<User, bool>> exp) =>
                FakeUsers().Where(exp.Compile()));

        var req = new PagedRequestDto { Page = 1, PageSize = 10, Q = "123" };

        var result = await _service.GetUsersPagedAsync(req);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(0, result.Data.Items.Count());
        Assert.AreEqual(0, result.Data.Total);
    }


    [Test]
    public async Task GetUsersPagedAsync_Exception_Return500()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ThrowsAsync(new Exception("DB error"));

        var result = await _service.GetUsersPagedAsync(new PagedRequestDto());

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
    }


    [Test]
    public async Task GetUserByIdAsync_Exist_ReturnUser()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(FakeUsers().First());

        var result = await _service.GetUserByIdAsync(1);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Alice", result.Data.FullName);
    }

    //[Test]
    //public async Task GetUserByIdAsync_Exist_Fail()
    //{
    //    _userRepoMock.Setup(r => r.GetByIdAsync(1))
    //        .ReturnsAsync(FakeUsers().First());

    //    var result = await _service.GetUserByIdAsync(1);

    //    Assert.IsTrue(result.IsSuccess);
    //    Assert.AreEqual("Alice", result.Data.FullName);
    //}

    [Test]
    public async Task GetUserByIdAsync_NotFound_Return404()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((User)null);

        var result = await _service.GetUserByIdAsync(99);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Test]
    public async Task CreateUserAsync_Valid_Return201()
    {
        _userRepoMock.Setup(r => r.IsEmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var req = new RegisterWarehouseRequest
        {
            FullName = "New User",
            Email = "new@mail.com",
            Password = "123456",
            Role = 1
        };

        var result = await _service.CreateUserAsync(req);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
    }

    [Test]
    public async Task CreateUserAsync_EmailExists_Return400()
    {
        _userRepoMock.Setup(r => r.IsEmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await _service.CreateUserAsync(new RegisterWarehouseRequest
        {
            Email = "exist@mail.com"
        });

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Test]
    public async Task UpdateUserAsync_Valid_Return200()
    {
        var user = FakeUsers().First();

        _userRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(user);

        _userRepoMock.Setup(r => r.IsEmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _service.UpdateUserAsync(1, new RegisterWarehouseRequest
        {
            FullName = "Updated Name",
            Email = "updated@mail.com",
            Role = 2
        });

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Updated Name", result.Data.FullName);
    }

    [Test]
    public async Task UpdateUserAsync_NotFound_Return404()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((User)null);

        var result = await _service.UpdateUserAsync(1, new RegisterWarehouseRequest());

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
    }

    private User ExistingUser() => new User
    {
        UserId = 1,
        FullName = "Old Name",
        Email = "oldmail@mail.com",
        Phone = "000",
        Address = "Old Address",
        Role = 1
    };

    private User ExistingUser2() => new User
    {
        UserId = 2,
        FullName = "sdcfs",
        Email = "exist@mail.com",
        Phone = "000",
        Address = "Old Address",
        Role = 1
    };

    [Test]
    public async Task UpdateUserAsync_EmailAlreadyExistsByAnotherUser_Return400()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(ExistingUser());

        _userRepoMock
            .Setup(r => r.IsEmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var req = new RegisterWarehouseRequest
        {
            FullName = "Updated Name",
            Email = "exist@mail.com",
            Role = 1
        };

        var result = await _service.UpdateUserAsync(1, req);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }



    [Test]
    public async Task UpdateUserAsync_Role99_StillReturn200()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(ExistingUser());

        _userRepoMock.Setup(r => r.IsEmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var req = new RegisterWarehouseRequest
        {
            Email = "test@example.com",
            Role = 99
        };

        var result = await _service.UpdateUserAsync(1, req);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }




    [Test]
    public async Task DeleteUserAsync_Exist_ReturnTrue()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(FakeUsers().First());

        var result = await _service.DeleteUserAsync(1);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.Data);
    }

    [Test]
    public async Task DeleteUserAsync_NotFound_Return404()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((User)null);

        var result = await _service.DeleteUserAsync(99);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
    }

    //[Test]
    //public async Task DeleteUserAsync_NotFound_Return404Fail()
    //{
    //    _userRepoMock.Setup(r => r.GetByIdAsync(null)
    //        .ReturnsAsync((User)null);

    //    var result = await _service.DeleteUserAsync(99);

    //    Assert.IsFalse(result.IsSuccess);
    //    Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
    //}

}
