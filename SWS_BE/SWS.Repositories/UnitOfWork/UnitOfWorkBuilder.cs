using SWS.BusinessObjects.Models;

namespace SWS.Repositories.UnitOfWork
{
    public class UnitOfWorkBuilder
    {
        private SmartWarehouseDbContext? _context;

        public UnitOfWorkBuilder WithContext(SmartWarehouseDbContext context)
        {
            _context = context;
            return this;
        }

        public UnitOfWorkBuilder WithAccountRepository()
        {
            return this;
        }

        public UnitOfWorkBuilder WithRoleRepository()
        {
            return this;
        }

        public UnitOfWork Build()
        {
            if (_context == null)
                throw new InvalidOperationException("Context must be set before building UnitOfWork");
            
            return new UnitOfWork(_context);
        }
    }
}
