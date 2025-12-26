using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace SWS.Test.GoogleSheet
{
    public class GoogleSheetService
    {
        private readonly SheetsService _service;
        private readonly string _spreadsheetId;

        public GoogleSheetService(string spreadsheetId)
        {
            var credentialPath =
                Environment.GetEnvironmentVariable("GOOGLE_SHEET_CREDENTIALS");

            if (string.IsNullOrEmpty(credentialPath))
                throw new Exception("GOOGLE_SHEET_CREDENTIALS env var not set");

            GoogleCredential credential =
                GoogleCredential
                    .FromFile(credentialPath)
                    .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);

            _service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "SWS Automation Test"
            });

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
