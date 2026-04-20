using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskify.Core.Entities;

namespace Taskify.Core.Repositories
{
    public interface ITaskRepository
    {
        public Task<List<Tasks>> GetAllTasksAsync();
        public Task<List<Tasks>> GetAllTasksByWorkerAsync(int userId);
        public Task<List<Tasks>> GetTasksByProjectIdAsync(int projectId);
        public Task<Tasks> GetTaskByIdAsync(int taskId);
        
        public Tasks AddTaskAsync(Tasks task); 
        public Task<Tasks> UpdateTaskAsync(Tasks task);
        public Task<Tasks> UpdateTaskWorkerAsync(Tasks task);

        public Task<Tasks> DeleteTaskAsync(int taskId);
        public Task SaveAsync();

    }
}
