using AutoMapper;
using Moq;
using NUnit.Framework;
using SWS.BusinessObjects.DTOs;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Repositories.BusinessPartners;
using SWS.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class BusinessPartnerServiceTest
{
    private Mock<IBusinessPartnerRepository> _repoMock;
    private Mock<IMapper> _mapperMock;
    private BusinessPartnerService _service;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IBusinessPartnerRepository>();
        _mapperMock = new Mock<IMapper>();

        _service = new BusinessPartnerService(
            _repoMock.Object,
            _mapperMock.Object
        );
    }

    private List<BusinessPartner> FakePartners() => new()
    {
        new BusinessPartner { PartnerId = 1, Name = "Partner A", Type = "Supplier" },
        new BusinessPartner { PartnerId = 2, Name = "Partner B", Type = "Supplier" }
    };

    private List<BusinessPartnerDto> FakePartnerDtos() => new()
    {
        new BusinessPartnerDto { PartnerId = 1, Name = "Partner A" },
        new BusinessPartnerDto { PartnerId = 2, Name = "Partner B" }
    };

    [Test]
    public async Task GetPartnersByTypeAsync_ValidType_ReturnsMappedDtos()
    {
        // Arrange
        var partners = FakePartners();
        var partnerDtos = FakePartnerDtos();

        _repoMock
            .Setup(r => r.GetPartnersByTypeAsync("Supplier"))
            .ReturnsAsync(partners);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<BusinessPartnerDto>>(partners))
            .Returns(partnerDtos);

        // Act
        var result = await _service.GetPartnersByTypeAsync("Supplier");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual("Partner A", result.First().Name);

        _repoMock.Verify(r => r.GetPartnersByTypeAsync("Supplier"), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<BusinessPartnerDto>>(partners), Times.Once);
    }

    [Test]
    public async Task GetPartnersByTypeAsync_NoData_ReturnsEmptyList()
    {
        // Arrange
        var emptyPartners = new List<BusinessPartner>();
        var emptyDtos = new List<BusinessPartnerDto>();

        _repoMock
            .Setup(r => r.GetPartnersByTypeAsync("Customer"))
            .ReturnsAsync(emptyPartners);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<BusinessPartnerDto>>(emptyPartners))
            .Returns(emptyDtos);

        // Act
        var result = await _service.GetPartnersByTypeAsync("Customer");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }



}
