using System.Collections.Generic;
using System.Threading.Tasks;
using Taskify.Core.Entities;

namespace Taskify.Core.Repositories
{
    public interface IUserRepository
    {
        public Task<List<User>> GetUsersAsync();

        public Task<User> GetUserByIDAsync(int id);

        public Task<User> AddUserAsync(User user);

        public Task<User> UpdateUserAsync(User user);

        public Task DeleteUserAsync(int id);

        public Task SaveAsync();
    }
}
