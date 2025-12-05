// File: SWS.Repositories/UnitOfWork/UnitOfWork.cs
using System.Threading.Tasks;

using SWS.BusinessObjects.Models;

using SWS.Repositories.Repositories.AccountRepo;
using SWS.Repositories.Repositories.ExportDetailRepo;
using SWS.Repositories.Repositories.ExportRepo;
using SWS.Repositories.Repositories.ProductRepo;
using SWS.Repositories.Repositories.UserRepo;

using SWS.Repositories.Repositories.ImportOrders;
using SWS.Repositories.Repositories.ReturnRepo;
using SWS.Repositories.Repositories.LocationRepo;
using SWS.Repositories.Repositories.InventoryRepo;
using SWS.Repositories.Repositories.CycleCountRepo;
using SWS.Repositories.Repositories.CycleCountDetailRepo;
using SWS.Repositories.Repositories.ActionLogRepo;
using SWS.Repositories.Repositories.TransactionLogRepo;

namespace SWS.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SmartWarehouseDbContext _context;

        // Repos sẵn có
        private IAccountRepository? _accountRepository;
        private IUserRepository? _userRepository;
        private IProductRepository? _productRepository;
        private IInventoryRepository? _inventoryRepository;
        private ILocationRepository? _locationRepository;

        // Import
        private IImportOrderQueryRepository? _importOrderQueryRepository;
        private IImportOrderCommandRepository? _importOrderCommandRepository;

        // Return
        private IReturnReasonRepository? _returnReasonRepository;
        private IReturnStatusRepository? _returnStatusRepository;
        private IReturnOrderQueryRepository? _returnOrderQueryRepository;
        private IReturnOrderCommandRepository? _returnOrderCommandRepository;

        // Export
        private IExportOrderRepository? _exportOrderRepository;
        private IExportDetailRepository? _exportDetailRepository;

        private ICycleCountRepository? _cycleCountRepository;
        private ICycleCountDetailRepository? _cycleCountDetailRepository;
        private IActionLogRepository? _actionLogRepository;
        private ITransactionLogRepository? _transactionLogRepository;

        public UnitOfWork(SmartWarehouseDbContext context)
        {
            _context = context;
        }

        // Sẵn có
        public IAccountRepository Accounts =>
            _accountRepository ??= new AccountRepository(_context);

        public IUserRepository Users =>
            _userRepository ??= new UserRepository(_context);

        public IProductRepository Products =>
            _productRepository ??= new ProductRepository(_context);
        
        public ILocationRepository Locations =>
            _locationRepository ??= new LocationRepository(_context);
        
        public IExportOrderRepository ExportOrders =>
            _exportOrderRepository ??= new ExportOrderRepository(_context);

        public IExportDetailRepository ExportDetails =>
            _exportDetailRepository ??= new ExportDetailRepository(_context);

        // Import
        public IImportOrderQueryRepository ImportOrdersQuery =>
            _importOrderQueryRepository ??= new ImportOrderQueryRepository(_context);

        public IImportOrderCommandRepository ImportOrdersCommand =>
            _importOrderCommandRepository ??= new ImportOrderCommandRepository(_context);

        // Return
        public IReturnReasonRepository ReturnReasons =>
            _returnReasonRepository ??= new ReturnReasonRepository(_context);

        public IReturnStatusRepository ReturnStatuses =>
            _returnStatusRepository ??= new ReturnStatusRepository(_context);

        public IReturnOrderQueryRepository ReturnOrdersQuery =>
            _returnOrderQueryRepository ??= new ReturnOrderQueryRepository(_context);

        public IReturnOrderCommandRepository ReturnOrdersCommand =>
            _returnOrderCommandRepository ??= new ReturnOrderCommandRepository(_context);

        public ICycleCountRepository CycleCounts => 
            _cycleCountRepository ??= new CycleCountRepository(_context);

        public ICycleCountDetailRepository CycleCountDetails => 
            _cycleCountDetailRepository ??= new CycleCountDetailRepository(_context);

        public IInventoryRepository Inventories => 
            _inventoryRepository ??= new InventoryRepository(_context);
        
        public IActionLogRepository ActionLogs => 
            _actionLogRepository ??= new ActionLogRepository(_context);
        
        public ITransactionLogRepository TransactionLogs => 
            _transactionLogRepository ??= new TransactionLogRepository(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context?.Dispose();
    }
}
