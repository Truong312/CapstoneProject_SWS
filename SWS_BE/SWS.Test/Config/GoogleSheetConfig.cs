using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Test.Config
{
    public static class GoogleSheetConfig
    {
        // Path to service account credentials
        public static string CredentialPath =>
            Path.Combine(AppContext.BaseDirectory, "Config", "credentials.json");

        // Spreadsheet ID (from Google Sheet URL)
        public const string SpreadsheetId =
            "1AbCdEfGhIjKlMnOpQrStUvWxYz"; // <-- replace

        // Sheet names (tabs)
        public const string ResetPasswordSheet = "resetPassword";

        // Fixed positions (based on your sheet format)
        public const int TestCaseHeaderRow = 6;   // row 7
        public const int TestCaseStartCol = 5;    // column F

        public const int ConditionStartRow = 7;   // row 8 (A8)
    }
}
