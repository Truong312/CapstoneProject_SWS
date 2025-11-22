using SWS.Services.ApiModels.SqlConverts;
using SWS.Services.ApiModels.Commons;

namespace SWS.Services.ConvertSqlRawServices;

public interface ITextToSqlService
{
    Task<ResultModel<SqlQueryResultDto>> QueryAsync(string naturalLanguage, CancellationToken ct = default);
}
