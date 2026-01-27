using System.Collections.Generic;
using System.Threading.Tasks;
using Taskify.Core.Entities;

namespace Taskify.Core.Repositories
{
    public interface ITaskRepository
    {
        public Task<List<Tasks>> GetAllTasksAsync();

        public Task<Tasks> GetTaskByIdAsync(int id);

        public Task<Tasks> AddTaskAsync(Tasks task);

        public Task<Tasks> UpdateTaskAsync(Tasks task);

        public Task DeleteTaskAsync(int id);

        public Task SaveAsync();
    }
}
