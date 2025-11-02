using SWS.BusinessObjects.Models;
using SWS.Repositories.Repositories.AccountRepo;
using SWS.Repositories.Repositories.ExportDetailRepo;
using SWS.Repositories.Repositories.ExportRepo;
using SWS.Repositories.Repositories.ProductRepo;
using SWS.Repositories.Repositories.UserRepo;

namespace SWS.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SmartWarehouseDbContext _context;
        private IAccountRepository? _accountRepository;
        private IUserRepository? _userRepository;
        private IProductRepository? _productRepository;
        private IExportOrderRepository? _exportOrderRepository;
        private IExportDetailRepository? _exportDetailRepository;

        public UnitOfWork(SmartWarehouseDbContext context)
        {
            _context = context;
        }

        public IAccountRepository Accounts => _accountRepository ??= new AccountRepository(_context);
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);
        public IExportOrderRepository ExportOrders => _exportOrderRepository ??= new ExportOrderRepository(_context);
        public IExportDetailRepository ExportDetails => _exportDetailRepository ??= new ExportDetailRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
