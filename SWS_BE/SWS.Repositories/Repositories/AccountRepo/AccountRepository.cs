using SWS.BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.AccountRepo
{
    public class AccountRepository : GenericRepository<User>, IAccountRepository
    {
        private readonly SmartWarehouseDbContext _context;

        public AccountRepository(SmartWarehouseDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Email == usernameOrEmail);
        }

        public async Task<List<string>> GetRoleNamesByAccountIdAsync(int accountId)
        {
            // SmartWarehouse uses User entity with Role as int, not separate Account/Role tables
            // Return role as string for compatibility
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == accountId);
            
            if (user == null)
                return new List<string>();
            
            return new List<string> { user.Role.ToString() };
        }

        public async Task<List<int>> GetRolesByAccountIdAsync(int accountId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == accountId);
            
            if (user == null)
                return new List<int>();
            
            return new List<int> { user.Role };
        }

        public async Task AddAccountRoleAsync(int userId, int role)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Role = role;
                await _context.SaveChangesAsync();
            }
        }
    }
}
