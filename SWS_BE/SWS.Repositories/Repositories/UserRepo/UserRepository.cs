using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace SWS.Repositories.Repositories.UserRepo
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly SmartWarehouseDbContext _context;

        public UserRepository(SmartWarehouseDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdWithDetailsAsync(int userId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email);
        }
    }
}

