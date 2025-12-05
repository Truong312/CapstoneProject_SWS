using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Enums;
using SWS.Services.ApiModels.Commons;

namespace SWS.Services.Services.LogServices
{
    public interface ITransactionLogService
    {
        public Task<ResultModel> CreateTransactionLogAsync(int orderId, int productId, int quantity, TransactionType type, string? note, int quantityChange);
    }
}
