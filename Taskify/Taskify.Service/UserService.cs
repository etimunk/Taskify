using System.Collections.Generic;
using System.Threading.Tasks;
using Taskify.Core.Entities;
using Taskify.Core.Repositories;
using Taskify.Core.Servieces;

namespace Taskify.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task<User> GetUserByIDAsync(int id)
        {
            return await _userRepository.GetUserByIDAsync(id);
        }

        public async Task<User> AddUserAsync(User user)
        {
            var u = await _userRepository.AddUserAsync(user);
            await _userRepository.SaveAsync();
            return u;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var updatedUser = await _userRepository.UpdateUserAsync(user);
            if (updatedUser != null)
            {
                await _userRepository.SaveAsync();
            }
            return updatedUser;
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
            await _userRepository.SaveAsync();
        }
    }
}