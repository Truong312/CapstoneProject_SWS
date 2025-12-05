using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Enums;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.TransactionLogRepo
{
    public interface ITransactionLogRepository:IGenericRepository<TransactionLog>
    {
        Task<IEnumerable<TransactionLog>> GetWithCondition(DateOnly? startDate, DateOnly? endDate, TransactionType? transactionType);
    }
}
