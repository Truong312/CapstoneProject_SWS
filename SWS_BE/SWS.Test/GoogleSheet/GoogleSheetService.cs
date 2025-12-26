using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace SWS.Test.GoogleSheet
{
    public class GoogleSheetService
    {
        private readonly SheetsService _service;
        private readonly string _spreadsheetId;

        public GoogleSheetService(string credentialPath, string spreadsheetId)
        {
            // Load credentials
            GoogleCredential credential = GoogleCredential
                .FromFile(credentialPath)
                .CreateScoped(SheetsService.Scope.Spreadsheets);

            // Initialize SheetsService
            _service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "AutomationTest"
            });

            // Save spreadsheet id
            _spreadsheetId = spreadsheetId;
        }

        public IList<IList<object>> ReadSheet(string sheetName)
        {
            var request = _service.Spreadsheets.Values.Get(
                _spreadsheetId,
                sheetName
            );

            return request.Execute().Values ?? new List<IList<object>>();
        }
    }
}
