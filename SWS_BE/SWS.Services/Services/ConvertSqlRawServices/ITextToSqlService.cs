using SWS.Services.ApiModels.SqlConverts;
using SWS.Services.ApiModels;

namespace SWS.Services.ConvertSqlRawServices;

public interface ITextToSqlService
{
    Task<ResultModel<SqlQueryResultDto>> QueryAsync(string naturalLanguage, CancellationToken ct = default);
}
