// File: SWS.Repositories/UnitOfWork/IUnitOfWork.cs
using System;
using System.Threading.Tasks;
using SWS.Repositories.Repositories.LocationRepo;
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
using SWS.Repositories.Repositories.ActionLogRepo;
using SWS.Repositories.Repositories.TransactionLogRepo;

namespace SWS.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Đang có
        IAccountRepository Accounts { get; }
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        IInventoryRepository Inventories { get; }
        // Location / Warehouse layout
        ILocationRepository Locations { get; }

        // Import
        IImportOrderQueryRepository ImportOrdersQuery { get; }
        IImportOrderCommandRepository ImportOrdersCommand { get; }

        // Return (lookup + query)
        IReturnReasonRepository ReturnReasons { get; }
        IReturnStatusRepository ReturnStatuses { get; }
        IReturnOrderQueryRepository ReturnOrdersQuery { get; }

        // ➕ Return (command) — dùng cho Review
        IReturnOrderCommandRepository ReturnOrdersCommand { get; }

        // Export
        IExportOrderRepository ExportOrders { get; }
        IExportDetailRepository ExportDetails { get; }
        ICycleCountRepository CycleCounts { get; }
        ICycleCountDetailRepository CycleCountDetails { get; }
        IActionLogRepository ActionLogs { get; }
        ITransactionLogRepository TransactionLogs { get; }
        Task<int> SaveChangesAsync();
    }
}
