using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using SWS.Test.Config;

namespace SWS.Test.GoogleSheet
{
    public class GoogleSheetService
    {
        private readonly SheetsService _service;
        private readonly string _spreadsheetId;

        public GoogleSheetService(string spreadsheetId)
        {
            _spreadsheetId = spreadsheetId;

            var credentialPath = GoogleSheetConfig.CredentialPath
                ?? throw new Exception("GOOGLE_SHEET_CREDENTIALS is not set");

            GoogleCredential credential;
            using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(SheetsService.Scope.Spreadsheets);
            }

            _service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "SWS Automation Test"
            });
        }

        /// <summary>
        /// Read all data from a sheet (A:Z)
        /// </summary>
        public IList<IList<object>> ReadSheet(string sheetName)
        {
            if (string.IsNullOrWhiteSpace(sheetName))
                throw new ArgumentException("sheetName is required");

            string range = $"{sheetName}!A:Z";

            var request = _service.Spreadsheets.Values.Get(_spreadsheetId, range);
            var response = request.Execute();

            return response.Values ?? new List<IList<object>>();
        }

        /// <summary>
        /// Update a single row
        /// </summary>
        public void UpdateRange(string range, IList<IList<object>> values)
        {
            var valueRange = new ValueRange
            {
                Values = values
            };

            var updateRequest = _service.Spreadsheets.Values.Update(
                valueRange,
                _spreadsheetId,
                range
            );

            updateRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            updateRequest.Execute();
        }
    }
}
