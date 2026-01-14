using AutoMapper;
using CloudinaryDotNet.Actions;
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
    public abstract class BaseAutomationTest
    {
        protected const string SheetId = "1SSGPplUNAbgzIG8okaG_9iHcYQ4_gkfIxnfU5Ejr2B0";

        protected IEnumerable<TestCase> GetTestCasesFromSheet(string sheetName)
        {
            var sheetService = new GoogleSheetService(SheetId);
            var sheet = sheetService.ReadSheet(sheetName);

            if (sheet == null || sheet.Count <= 1)
                return new List<TestCase>();

            // Log preview
            TestDebug.Log($"Total rows in sheet '{sheetName}': {sheet.Count}");

            var parser = new SheetParser(sheet);
            var cases = parser.ReadTestCases();

            foreach (var tc in cases)
            {
                tc.SheetName = sheetName;   // ⭐ GÁN Ở ĐÂY
            }

            TestDebug.Log($"Total TestCases = {cases.Count}");
            foreach (var tc in cases)
                TestDebug.Log($"TC {tc.Id} Inputs={tc.Inputs.Count}");

            return cases;
        }

        protected void CheckConfirmAndWriteResult(TestCase tc, bool actualSuccess, string actualReturnMessage = null, Exception actualException = null)
        {
            var sheetService = new GoogleSheetService(SheetId);
            var sheet = sheetService.ReadSheet(tc.SheetName);

            string GetCellSafe(IList<IList<object>> s, int row, int col)
            {
                if (row < 0 || row >= s.Count) return null;
                var r = s[row];
                if (col < 0 || col >= r.Count) return null;
                return r[col]?.ToString();
            }

            // ===== FIND CONFIRM / RETURN / EXCEPTION BLOCK =====
            int confirmRow = SheetParser.TryFindRow(sheet, "Confirm", 0, 0);
            int returnRow = SheetParser.TryFindRow(sheet, "Return", 1, confirmRow + 1);
            int exceptionRow = SheetParser.TryFindRow(sheet, "Exception", 1, returnRow + 1);

            int confirmValueRow = confirmRow + 1;
            int returnValueRow = returnRow >= 0 ? returnRow + 1 : -1;
            int exceptionValueRow = exceptionRow >= 0 ? exceptionRow + 1 : -1;

            string expectedConfirmRaw = GetCellSafe(sheet, confirmValueRow, 1);
            string expectedReturn = returnValueRow >= 0 ? GetCellSafe(sheet, returnValueRow, 1) : null;
            string expectedException = exceptionValueRow >= 0 ? GetCellSafe(sheet, exceptionValueRow, 1) : null;

            bool? expectedConfirm = string.IsNullOrWhiteSpace(expectedConfirmRaw)
                        ? (bool?)null
                        : ParseBool(expectedConfirmRaw);

            // ===== VALIDATE =====
            bool isPassed = true;
            if (expectedConfirm.HasValue)
                isPassed = actualSuccess == expectedConfirm.Value;

            if (isPassed && !string.IsNullOrEmpty(expectedReturn))
                isPassed = actualReturnMessage?.Contains(expectedReturn) == true;

            if (isPassed && !string.IsNullOrEmpty(expectedException))
                isPassed = actualException?.Message?.Contains(expectedException) == true;

            // ===== FIND OR ADD RESULT ROW =====
            int resultLabelRow = SheetParser.TryFindRow(sheet, "Result");
            if (resultLabelRow < 0) { sheet.Add(new List<object> { "Result" }); resultLabelRow = sheet.Count - 1; }

            int pfRow = resultLabelRow + 1;       // Row P/F
            int executedRow = pfRow + 1;          // Row Executed Date
            int defectRow = executedRow + 1;      // Row Defect ID
            int col = tc.ColumnIndex;

            // ===== ENSURE SHEET ROWS EXIST =====
            while (sheet.Count <= defectRow) sheet.Add(new List<object>());

            // ===== ENSURE COLUMNS =====
            EnsureColumnCount(sheet[pfRow], col + 1);
            EnsureColumnCount(sheet[executedRow], col + 1);
            EnsureColumnCount(sheet[defectRow], col + 1);

            // ===== WRITE P/F, Executed Date, Defect ID =====
            sheet[pfRow][col] = isPassed ? "P" : "F";
            sheet[executedRow][col] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            sheet[defectRow][col] = !isPassed ? $"{GenerateNextDefectId(sheet)} {actualException?.Message ?? actualReturnMessage}" : "";

            // ===== GHI HỆ THỐNG CHẠY L2 =====
            while (sheet.Count <= 1) sheet.Add(new List<object>());
            EnsureColumnCount(sheet[1], 12);
            sheet[1][11] = "System Run";

            // ===== UPDATE RANGE =====
            sheetService.UpdateRange(
                $"{tc.SheetName}!A{pfRow + 1}:Z{defectRow + 1}",
                sheet.Skip(pfRow).Take(defectRow - pfRow + 1).ToList()
            );

            TestDebug.Log($"[DEBUG] ✅ {tc.Id} → {(isPassed ? "PASS" : "FAIL")}" + (isPassed ? "" : " (Defect created)"));
            Assert.That(isPassed, Is.True);
        }


        protected ConfirmResult ReadConfirmBlock( IList<IList<object>> sheet, int startRow = 0)
        {
            // Find rows by label + column
            int confirmRow = SheetParser.FindRow(sheet, "Confirm");
            int returnRow = SheetParser.FindRow(sheet, "Return", confirmRow + 1);
            int exceptionRow = SheetParser.FindRow(sheet, "Exception", returnRow + 1);

            // Value rows = next row
            int confirmValueRow = confirmRow + 1;
            int returnValueRow = returnRow + 1;
            int exceptionValueRow = exceptionRow + 1;

            string confirmRaw = SheetParser.GetCell(sheet, confirmValueRow, 3);
            string returnRaw = SheetParser.GetCell(sheet, returnValueRow, 3);
            string exception = SheetParser.GetCell(sheet, exceptionValueRow, 3);

            bool expectedConfirm = ParseBool(confirmRaw);

            return new ConfirmResult
            {
                ExpectedConfirm = expectedConfirm,
                ExpectedReturn = returnRaw,
                ExpectedException = exception
            };
        }

        protected void WriteTestResultToSheet(TestCase tc, bool isPassed, string defectReason = null)
        {
            var sheetService = new GoogleSheetService(SheetId);
            var sheet = sheetService.ReadSheet(tc.SheetName);

            int resultLabelRow = SheetParser.FindRow(sheet, "Result");

            int passFailRow = resultLabelRow + 1;
            int executedRow = resultLabelRow + 2;
            int defectIdRow = resultLabelRow + 3;

            int col = tc.ColumnIndex; // thường là cột B = 1

            EnsureColumnCount(sheet[passFailRow], col + 1);
            EnsureColumnCount(sheet[executedRow], col + 1);
            EnsureColumnCount(sheet[defectIdRow], col + 1);

            // 1️⃣ PASS / FAIL
            sheet[passFailRow][col] = isPassed ? "P" : "F";

            // 2️⃣ Executed Date
            sheet[executedRow][col] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 3️⃣ Defect ID
            if (!isPassed)
            {
                var defectId = GenerateNextDefectId(sheet);
                sheet[defectIdRow][col] =
                    $"{defectId} {defectReason ?? "Unknown error"}";
            }
            else
            {
                sheet[defectIdRow][col] = "";
            }

            sheetService.UpdateRange(
                $"{tc.SheetName}!A{passFailRow + 1}:Z{defectIdRow + 1}",
                new List<IList<object>> { sheet[passFailRow], sheet[executedRow], sheet[defectIdRow] });

            TestDebug.Log(
                $"✅ {tc.Id} → {(isPassed ? "PASS" : "FAIL")} {(isPassed ? "" : "(Defect created)")}"
            );
        }


        protected string GenerateNextDefectId( IList<IList<object>> sheet)
        {
            int max = 0;

            foreach (var row in sheet)
            {
                foreach (var cell in row)
                {
                    var text = cell?.ToString();
                    if (text == null || !text.StartsWith("DF"))
                        continue;

                    if (int.TryParse(text.Substring(2), out int n))
                        max = Math.Max(max, n);
                }
            }

            return $"DF{(max + 1):D3}";
        }


        protected static void EnsureColumnCount( IList<object> row, int requiredCount)
        {
            while (row.Count < requiredCount)
                row.Add("");
        }

        protected static bool ParseBool(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return value.Equals("true", StringComparison.OrdinalIgnoreCase)
                || value.Equals("yes", StringComparison.OrdinalIgnoreCase)
                || value.Equals("1");
        }

        protected (int? userId, string oldPassword, string newPassword)
            ParseInputs(Dictionary<string, string> inputs)
        {
            int? userId = null;

            if (inputs.TryGetValue("userId", out var raw) &&
                !string.IsNullOrWhiteSpace(raw) &&
                !raw.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                userId = int.Parse(raw);
            }

            return (
                userId,
                inputs.GetValueOrDefault("oldPassword"),
                inputs.GetValueOrDefault("newPassword")
            );
        }

        protected WarehouseAuthenticationService SetupWarehouseAuthMocks(
            int? userId,
            string oldPassword,
            out bool userExists)
        {
            var userRepoMock = new Mock<IUserRepository>();

            if (userId.HasValue && userId != 999)
            {
                var user = new User
                {
                    UserId = userId.Value,
                    Password = PasswordHelper.HashPassword(oldPassword)
                };

                userRepoMock.Setup(x => x.GetByIdAsync(userId.Value))
                            .ReturnsAsync(user);
                userRepoMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                            .Returns(Task.CompletedTask);
            }
            else
            {
                userRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync((User)null);
            }

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(x => x.Users).Returns(userRepoMock.Object);
            uowMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var actionLogMock = new Mock<IActionLogService>();
            actionLogMock.Setup(x =>
                x.CreateActionLogAsync(It.IsAny<ActionType>(), It.IsAny<string>(), It.IsAny<string>())
            ).ReturnsAsync(new ResultModel { IsSuccess = true });

            userExists = userId.HasValue && userId != 999;

            return new WarehouseAuthenticationService(
                uowMock.Object,
                Mock.Of<IConfiguration>(),
                Mock.Of<IGoogleLoginService>(),
                Mock.Of<IMapper>(),
                actionLogMock.Object
            );
        }
    }
}
