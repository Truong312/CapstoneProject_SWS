using SWS.BusinessObjects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.AccountRepo
{
    public interface IAccountRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<List<string>> GetRoleNamesByAccountIdAsync(int accountId);
        Task<List<int>> GetRolesByAccountIdAsync(int accountId);
        Task AddAccountRoleAsync(int userId, int role);
    }
}
