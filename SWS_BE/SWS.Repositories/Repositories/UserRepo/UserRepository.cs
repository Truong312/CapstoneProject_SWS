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

        public async Task<(IEnumerable<User> items, int totalCount)> GetPagedUsersAsync(
            int pageIndex,
            int pageSize,
            string? search = null,
            int? roleFilter = null,
            string? sortBy = null,
            bool sortDesc = false)
        {
            IQueryable<User> query = _context.Users.AsNoTracking();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(u => 
                    u.FullName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search) ||
                    (u.Phone != null && u.Phone.Contains(search)));
            }

            // Apply role filter
            if (roleFilter.HasValue)
            {
                query = query.Where(u => u.Role == roleFilter.Value);
            }

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "fullname" => sortDesc ? query.OrderByDescending(u => u.FullName) : query.OrderBy(u => u.FullName),
                "email" => sortDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "role" => sortDesc ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
                _ => query.OrderBy(u => u.UserId) // Default sorting
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
