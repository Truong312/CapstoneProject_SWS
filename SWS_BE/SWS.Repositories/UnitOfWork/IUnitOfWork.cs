using SWS.Repositories.Repositories.AccountRepo;
using SWS.Repositories.Repositories.ProductRepo;
using SWS.Repositories.Repositories.UserRepo;
using SWS.Repositories.Repositories.ImportOrders;
using SWS.Repositories.Repositories.ReturnRepo;

namespace SWS.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Đang có
        IAccountRepository Accounts { get; }
        IUserRepository Users { get; }
        IProductRepository Products { get; }

        // ➕ Thêm cho Import
        IImportOrderQueryRepository ImportOrdersQuery { get; }
        IImportOrderCommandRepository ImportOrdersCommand { get; }

        // ➕ Thêm cho Return
        IReturnReasonRepository ReturnReasons { get; }
        IReturnStatusRepository ReturnStatuses { get; }
        IReturnOrderQueryRepository ReturnOrdersQuery { get; }

        Task<int> SaveChangesAsync();
    }
}
