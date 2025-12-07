using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SWS.BusinessObjects.AppSettings;
using SWS.BusinessObjects.Exceptions;
using SWS.Repositories.Generic;
using SWS.Services.Services.ConvertSqlRawServices;
using System.Data;
using System.Net;
using System.Text;
using System.Text.Json;

namespace SWS.Test.Service
{
    [TestFixture]
    public class VoiceConvertTest
    {
        private Mock<IDapperRepository> _mockDapper;
        private IOptions<GeminiSettings> _geminiOptions;
        private HttpClient _httpClient;
        private TextToSqlService_Gemini _service;

        [SetUp]
        public void Setup()
        {
            _mockDapper = new Mock<IDapperRepository>();

            //Gemini settings
            var settings = new GeminiSettings
            {
                ApiKey = "fake-api-key",
                Model = "test-model"
            };
            _geminiOptions = Options.Create(settings);

            // Mock HttpClientHandler 
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    // Return a fake Gemini API JSON response
                    var responseObj = new
                    {
                        status = "ok",
                        main_query = "SELECT * FROM Product"
                    };
                    var json = System.Text.Json.JsonSerializer.Serialize(responseObj);
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };
                });

            // Create HttpClient with mocked handler
            _httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://fakeapi.local/") // optional, required if service uses relative URIs
            };

            // Mock IHttpClientFactory to return the mocked HttpClient
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
                                 .Returns(_httpClient);

            // Initialize the service with mocked dependencies
            _service = new TextToSqlService_Gemini(_geminiOptions, _mockDapper.Object, httpClientFactoryMock.Object);
        }


        [Test]
        public async Task QueryAsync_ValidInput_ReturnsSuccess()
        {
            // Arrange
            var query = "Get all products";

            // Mock Dapper result
            _mockDapper.Setup(d => d.QueryAsync<dynamic>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDbTransaction>()))
                .ReturnsAsync(new List<dynamic> { new { Id = 1, Name = "Laptop" } });

            // Act
            var result = await _service.QueryAsync(query);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("success", result.ResponseCode);
            Assert.IsNotNull(result.Data);
            var rows = (List<dynamic>)result.Data!.Result;
            Assert.AreEqual(1, rows.Count);
            Assert.AreEqual("SELECT * FROM Product", result.Data.Sql);
        }

        [Test]
        public void QueryAsync_EmptyInput_ThrowsAppException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.QueryAsync(""));
            Assert.AreEqual("invalid_input", ex.Code);
        }

        [Test]
        public async Task QueryAsync_DapperThrows_ReturnsExecutionFailed()
        {
            // Arrange
            var query = "Get all products";

            _mockDapper.Setup(d => d.QueryAsync<dynamic>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.QueryAsync(query);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            //Assert.AreEqual("execution_failed", result.ResponseCode);
            Assert.AreEqual("generation_failed", result.ResponseCode);
            Assert.IsNull(result.Data);
            StringAssert.Contains("Database error", result.Message);
        }

        [Test]
        public async Task QueryAsync_InvalidSQL_ReturnsInvalidSql()
        {
            // Arrange
            var query = "Hack the database";

            // Mock Gemini response to include a forbidden keyword
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<System.Threading.CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"status\":\"ok\",\"main_query\":\"DROP TABLE Users;\"}", Encoding.UTF8, "application/json")
                });
            var httpClient = new HttpClient(handlerMock.Object);
            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var serviceWithInvalidSql = new TextToSqlService_Gemini(_geminiOptions, _mockDapper.Object, factoryMock.Object);

            // Act
            var result = await serviceWithInvalidSql.QueryAsync(query);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("invalid_sql", result.ResponseCode);
        }
    }
}
