using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskify.Core.Entities;

namespace Taskify.Core.Repositories
{
    public interface IUserRepository
    {
        public Task<List<User>> GetUsersAsync();
        public Task<User> GetUserByIDAsync(int id);
        public Task<User> GetUsersByEmailAndRoleAsync(string email,string role);
        public User AddUser(User user);
        public Task<User> UpdateUserAsync(User user);
        public Task<User> UpdateUserForHeadManagerAsync(User user);
        public Task<User> DeleteUserAsync(int id);
        public Task saveAsync();

    }
}
