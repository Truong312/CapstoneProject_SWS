using SWS.Repositories.Repositories.AccountRepo;
using SWS.Repositories.Repositories.ExportDetailRepo;
using SWS.Repositories.Repositories.ExportRepo;
using SWS.Repositories.Repositories.ProductRepo;
using SWS.Repositories.Repositories.UserRepo;
using SWS.Repositories.Repositories.ImportOrders;
using SWS.Repositories.Repositories.ReturnRepo;
using SWS.Repositories.Repositories.CycleCountRepo;
using SWS.Repositories.Repositories.CycleCountDetailRepo;
using SWS.Repositories.Repositories.InventoryRepo;

namespace SWS.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Đang có
        IAccountRepository Accounts { get; }
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        IInventoryRepository Inventories { get; }

        // ➕ Thêm cho Import
        IImportOrderQueryRepository ImportOrdersQuery { get; }
        IImportOrderCommandRepository ImportOrdersCommand { get; }

        // ➕ Thêm cho Return
        IReturnReasonRepository ReturnReasons { get; }
        IReturnStatusRepository ReturnStatuses { get; }
        IReturnOrderQueryRepository ReturnOrdersQuery { get; }

        IExportOrderRepository ExportOrders { get; }
        IExportDetailRepository ExportDetails { get; }

        ICycleCountRepository CycleCounts { get; }
        ICycleCountDetailRepository CycleCountDetails { get; }
        Task<int> SaveChangesAsync();
    }
}
