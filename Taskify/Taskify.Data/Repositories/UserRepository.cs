using Microsoft.EntityFrameworkCore;
using Taskify.Core.Entities;
using Taskify.Core.Repositories;

namespace Taskify.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.users.ToListAsync();
        }

        public async Task<User> GetUserByIDAsync(int id)
        {
            return await _context.users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _context.users.AddAsync(user);
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var use = await _context.users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (use != null)
            {
                use.Name = user.Name;
                use.TZ = user.TZ;
                use.Email = user.Email;
                use.Password = user.Password;
                use.Level = user.Level;
                use.Role = user.Role;
            }
            return use;
        }

        public async Task DeleteUserAsync(int id)
        {
            var use = await _context.users.FirstOrDefaultAsync(u => u.Id == id);
            if (use != null)
            {
                _context.users.Remove(use);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}