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
namespace SWS.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SmartWarehouseDbContext _context;

        // Repos sẵn có
        private IAccountRepository? _accountRepository;
        private IUserRepository? _userRepository;
        private IProductRepository? _productRepository;

        // Import
        private IImportOrderQueryRepository? _importOrderQueryRepository;
        private IImportOrderCommandRepository? _importOrderCommandRepository;

        // Return
        private IReturnReasonRepository? _returnReasonRepository;
        private IReturnStatusRepository? _returnStatusRepository;
        private IReturnOrderQueryRepository? _returnOrderQueryRepository;

        // ➕ Return (command) — mới thêm
        private IReturnOrderCommandRepository? _returnOrderCommandRepository;

        // Export
        private IExportOrderRepository? _exportOrderRepository;
        private IExportDetailRepository? _exportDetailRepository;

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

        // ➕ Return (command) — dùng cho Review
        public IReturnOrderCommandRepository ReturnOrdersCommand =>
            _returnOrderCommandRepository ??= new ReturnOrderCommandRepository(_context);
        private ILocationRepository? _locationRepository;

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context?.Dispose();
    }
}
