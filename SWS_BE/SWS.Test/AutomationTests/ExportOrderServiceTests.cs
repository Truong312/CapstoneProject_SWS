//using NUnit.Framework;
//using SWS.Test.AutomationTests;
//using SWS.Test.Helpers;
//using SWS.Test.Models;
//using SWS.Services.Services.ExportOrderServices;
//using Moq;
//using SWS.Repositories.UnitOfWork;
//using SWS.BusinessObjects.Enums;
//using SWS.Repositories.Repositories.ExportRepo;

//namespace SWS.Test.AutomationTests.Service
//{
//    [TestFixture]
//    public class ExportOrderTests : BaseAutomationTest
//    {
//        [Test]
//        [TestCaseSource(nameof(GetTestCases))]
//        public async Task GetExportOrdersByStatus_Automation_Test(TestCase tc)
//        {
//            TestDebug.Log($"==== RUN TC {tc.Id} ====");

//            // 1️⃣ Parse input from Google Sheet
//            var statusStr = tc.Inputs.GetValueOrDefault("status");
//            var status = Enum.Parse<StatusEnums>(statusStr, true);

//            // 2️⃣ Mock repository and unit of work
//            var exportOrdersMockData = tc.Inputs.ContainsKey("mockData")
//                ? ParseExportOrders(tc.Inputs["mockData"])
//                : new List<BusinessObjects.Models.ExportOrder>();

//            var exportOrderRepoMock = new Mock<IExportOrderRepository>();
//            exportOrderRepoMock.Setup(r => r.GetAllAsync())
//                .ReturnsAsync(exportOrdersMockData);

//            var uowMock = new Mock<IUnitOfWork>();
//            uowMock.Setup(u => u.ExportOrders).Returns(exportOrderRepoMock.Object);

//            var service = new ExportOrderService(uowMock.Object);

//            bool actualSuccess = false;
//            string actualMessage = null;
//            Exception actualException = null;
//            int actualCount = 0;

//            // 3️⃣ Call service
//            try
//            {
//                var result = await service.GetExportOrdersByStatusAsync(status);

//                actualSuccess = result.IsSuccess;
//                actualMessage = result.Message;
//                actualCount = result.Data?.Count() ?? 0;
//            }
//            catch (Exception ex)
//            {
//                actualException = ex;
//                actualSuccess = false;
//            }

//            // 4️⃣ Compare with expected result from sheet
//            bool expectedSuccess = bool.Parse(tc.ExpectedResult.GetValueOrDefault("IsSuccess") ?? "true");
//            int expectedCount = int.Parse(tc.ExpectedResult.GetValueOrDefault("DataCount") ?? "0");

//            bool isPassed = actualSuccess == expectedSuccess && actualCount == expectedCount;

//            // 5️⃣ Ghi kết quả vào Google Sheet
//            CheckConfirmAndWriteResult(
//                tc,
//                isPassed,
//                actualMessage,
//                actualException
//            );
//        }

//        // 6️⃣ Parse mock data JSON từ sheet
//        private List<BusinessObjects.Models.ExportOrder> ParseExportOrders(string mockDataJson)
//        {
//            return System.Text.Json.JsonSerializer.Deserialize<List<BusinessObjects.Models.ExportOrder>>(mockDataJson)
//                   ?? new List<BusinessObjects.Models.ExportOrder>();
//        }

//        public static IEnumerable<TestCase> GetTestCases() =>
//            new ExportOrderTests()
//                .GetTestCasesFromSheet("ExportOrders");
//    }
//}
