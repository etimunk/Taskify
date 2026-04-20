using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
            return  await _userRepository.GetUsersAsync();
        } 
        public async Task<User> GetUserByIDAsync(int id)
        {
            return await _userRepository.GetUserByIDAsync(id);
        }
        public async Task<User> GetUsersByEmailAndRoleAsync(string email, string role)
        {
            return await _userRepository.GetUsersByEmailAndRoleAsync( email,  role);
        }
        public async Task<User> AddUserAsync(User user)
        {
            var addedUser = _userRepository.AddUser(user);
            await _userRepository.saveAsync();
            return addedUser;
        }
        public  async Task<User> UpdateUserAsync(User user)
        {
           var u=  await _userRepository.UpdateUserAsync(user);
            await _userRepository.saveAsync();
            return u;
        }
        public async Task<User> UpdateUserForHeadManagerAsync(User user)
        {
            var u = await _userRepository.UpdateUserForHeadManagerAsync(user);
            await _userRepository.saveAsync();
            return u;
        }
        public  async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
            await _userRepository.saveAsync();
        }
    }
}
