using NUnit.Framework;
using SWS.Test.AutomationTests;
using SWS.Test.Helpers;
using SWS.Test.Models;

namespace SWS.Test.AutomationTests.Service;
[TestFixture]
public class ResetPasswordTests : BaseAutomationTest
{
    [Test]
    [TestCaseSource(nameof(GetTestCases))]
    public async Task ChangePassword_Automation_Test(TestCase tc)
    {
        TestDebug.Log($"==== RUN TC {tc.Id} ====");

        // 1️⃣ Parse input
        var (userId, oldPassword, newPassword) = ParseInputs(tc.Inputs);

        // 2️⃣ Mock service
        bool userExists;
        var service = SetupWarehouseAuthMocks(
            userId,
            oldPassword,
            out userExists
        );

        bool actualSuccess = false;
        string actualMessage = null;
        Exception actualException = null;

        // 3️⃣ Call service
        try
        {
            var result = await service.ChangePasswordAsync(
                userId!.Value,
                oldPassword,
                newPassword
            );

            actualSuccess = result.IsSuccess;
            actualMessage = result.Message;
        }
        catch (Exception ex)
        {
            actualException = ex;
            actualSuccess = false;
        }

        // 4️⃣ GIAO CHO BASE XỬ LÝ
        CheckConfirmAndWriteResult(
            tc,
            actualSuccess,
            actualMessage,
            actualException
        );
    }

    public static IEnumerable<TestCase> GetTestCases() =>
        new ResetPasswordTests()
            .GetTestCasesFromSheet("resetPassword");
}

