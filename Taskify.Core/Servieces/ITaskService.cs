using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskify.Core.Entities;


namespace Taskify.Core.Servieces
{
    namespace Taskify.Core.Servieces
    {
        public interface ITaskService
        {
            public Task<List<Tasks>> GetAllTasksAsync();
            public Task<List<Tasks>> GetTasksByProjectIdAsync(int projectId);
            public Task<List<Tasks>> GetAllTasksByWorkerAsync(int userId);
            public Task<Tasks> GetTaskByIdAsync(int taskId);
            public Task AddTaskAsync(Tasks task);
            public Task<Tasks> UpdateTaskAsync(Tasks task);
            public Task<Tasks> UpdateTaskWorkerAsync(Tasks task);
            public Task DeleteTaskAsync(int taskId);
        }
    }
}
