using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskify.Core.Entities;

namespace Taskify.Core.Servieces
{
    public interface IUserService
    {
        public Task<List<User>> GetUsersAsync();
        public Task<User> GetUserByIDAsync(int id);
        public Task<User> AddUserAsync(User user);
        public Task<User> UpdateUserAsync(User user);
        public Task DeleteUserAsync(int id);
    }
}
