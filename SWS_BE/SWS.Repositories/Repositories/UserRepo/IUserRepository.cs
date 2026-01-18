using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace SWS.Repositories.Repositories.UserRepo
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdWithDetailsAsync(int userId);
        Task<bool> IsEmailExistsAsync(string email);
        Task<(IEnumerable<User> items, int totalCount)> GetPagedUsersAsync(
            int pageIndex,
            int pageSize,
            string? search = null,
            int? roleFilter = null,
            string? sortBy = null,
            bool sortDesc = false);
    }
}
