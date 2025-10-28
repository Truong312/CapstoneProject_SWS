using SWS.Repositories.Repositories.AccountRepo;
using SWS.Repositories.Repositories.UserRepo;

namespace SWS.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository Accounts { get; }
        IUserRepository Users { get; }
        Task<int> SaveChangesAsync();
    }
}
