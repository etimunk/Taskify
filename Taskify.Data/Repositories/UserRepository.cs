using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
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
            return await _context.users
                                 .Where(u => u.Id == id)
                                 .SingleOrDefaultAsync(); // <--- השתנה מ SingleAsync ל SingleOrDefaultAsync
        }
        public async Task<User> GetUsersByEmailAndRoleAsync(string email, string role)
        {
            var user = await _context.users
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user != null && user.Level.ToString().Equals(role, StringComparison.OrdinalIgnoreCase))
            {
                return user;
            }
            return null;
        }
        public User AddUser(User user)
        {
            _context.users.Add(user);
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var use = await GetUserByIDAsync(user.Id); // שולף את הישות המקורית
            if (use != null)
            {
                use.Name = user.Name;
                use.TZ = user.TZ;
                use.Email = user.Email;
                use.Password = user.Password;

                return use; // <--- תחזירי את הישות המעודכנת מה-DB!
            }
            return null;
        }

        public async Task<User> UpdateUserForHeadManagerAsync(User user)
        {
            var use = await GetUserByIDAsync(user.Id);
            if (use != null && use.Level.ToString() != "headmanager")
            {
                use.Level = user.Level;
                use.Role = user.Role;

                return use; // <--- תחזירי את הישות המעודכנת מה-DB!
            }
            return use;
        }
        public async  Task<User> DeleteUserAsync(int id)
        {
            var use =  await _context.users.FirstAsync(u => u.Id == id);
            if (use != null)
            {
                _context.users.Remove(use);
            }
            return use;
        }
        public  async Task saveAsync()
        {
           await _context.SaveChangesAsync();
        }

    }

}






