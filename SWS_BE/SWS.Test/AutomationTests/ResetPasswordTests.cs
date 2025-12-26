using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Repositories.UserRepo;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels.Commons;
using SWS.Services.Helpers;
using SWS.Services.Services.LogServices;
using SWS.Services.Services.WarehouseAuthentication;
using SWS.Test.GoogleSheet;
using SWS.Test.Helpers;
using SWS.Test.Models;


namespace SWS.Test.AutomationTests
{
    [TestFixture]
    public class ResetPasswordTests
    {
        public static IEnumerable<TestCase> GetTestCases()
        {
            var service = new GoogleSheetService(
                "Config/credentials.json",
                "1SSGPplUNAbgzIG8okaG_9iHcYQ4_gkfIxnfU5Ejr2B0");

            var sheet = service.ReadSheet("resetPassword");

            TestDebug.Log($"Total rows = {sheet.Count}");

            for (int i = 0; i < Math.Min(15, sheet.Count); i++)
            {
                TestDebug.Log(
                    $"Row {i}: " +
                    string.Join(" | ", sheet[i].Select(x => x?.ToString() ?? "NULL"))
                );
            }

            var parser = new SheetParser(sheet);
            var cases = parser.ReadTestCases();

            TestDebug.Log($"Total TestCases = {cases.Count}");

            foreach (var tc in cases)
            {
                TestDebug.Log($"TC {tc.Id} Inputs={tc.Inputs.Count}");
            }

            return cases;
        }




        [TestCaseSource(nameof(GetTestCases))]
        public async Task ChangePassword_Automation_Test(TestCase tc)
        {
            TestDebug.Log($"==== RUN TC {tc.Id} ====");

            foreach (var kv in tc.Inputs)
                TestDebug.Log($"INPUT {kv.Key}={kv.Value}");

            if (tc.Expected == null)
            {
                tc.Expected = new TestExpected();
                TestDebug.Log($"⚠️ TC {tc.Id} Expected was NULL → auto init");
            }


            // ===== SAFE PARSE INPUT =====

            int? userId = null;
            if (tc.Inputs.TryGetValue("userId", out var userIdRaw) &&
                !string.IsNullOrEmpty(userIdRaw) &&
                !userIdRaw.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                userId = int.Parse(userIdRaw);
            }

            string oldPassword = tc.Inputs.GetValueOrDefault("oldPassword");
            string newPassword = tc.Inputs.GetValueOrDefault("newPassword");

            // ===== MOCK SETUP =====

            var userRepoMock = new Mock<IUserRepository>();

            if (userId.HasValue && userId != 999)
            {
                var user = new User
                {
                    UserId = userId.Value,
                    Password = PasswordHelper.HashPassword(oldPassword)
                };

                userRepoMock
                    .Setup(x => x.GetByIdAsync(userId.Value))
                    .ReturnsAsync(user);

                userRepoMock
                    .Setup(x => x.UpdateAsync(It.IsAny<User>()))
                    .Returns(Task.CompletedTask);
            }
            else
            {
                userRepoMock
                    .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync((User)null);
            }

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(x => x.Users).Returns(userRepoMock.Object);
            unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var actionLogMock = new Mock<IActionLogService>();
            actionLogMock
                .Setup(x => x.CreateActionLogAsync(
                    It.IsAny<ActionType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new ResultModel { IsSuccess = true });

            var service = new WarehouseAuthenticationService(
                unitOfWorkMock.Object,
                Mock.Of<IConfiguration>(),
                Mock.Of<IGoogleLoginService>(),
                Mock.Of<IMapper>(),
                actionLogMock.Object
            );

            // ===== ACT + ASSERT =====

            try
            {
                if (!userId.HasValue)
                {
                    Assert.That(tc.Expected.Exception, Is.Not.Null,
                        $"TC {tc.Id} expects exception but userId is null");

                    TestContext.WriteLine($"{tc.Id} → PASS (userId is null as expected)");
                    return;
                }

                var result = await service.ChangePasswordAsync(
                    userId.Value,
                    oldPassword,
                    newPassword
                );

                bool expectedSuccess =
                    string.Equals(tc.Expected.ReturnValue, "true", StringComparison.OrdinalIgnoreCase);

                string expectedMessage =
                    tc.Expected.Exception ?? tc.Expected.ReturnValue ?? string.Empty;

                Assert.That(result.IsSuccess, Is.EqualTo(expectedSuccess));

                if (!string.IsNullOrEmpty(expectedMessage))
                {
                    Assert.That(result.Message, Does.Contain(expectedMessage));
                }


                TestContext.WriteLine($"{tc.Id} → PASS");
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(tc.Expected.Exception))
                {
                    Assert.That(ex.Message, Does.Contain(tc.Expected.Exception));
                    TestContext.WriteLine($"{tc.Id} → PASS (Expected Exception)");
                }
                else
                {
                    throw;
                }
            }
        }


    }

}
