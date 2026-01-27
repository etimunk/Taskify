using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskify.Core.Entities;


namespace Taskify.Core.Servieces
{
    public interface ITaskService
    {
        public Task<List<Tasks>> GetAllTasksAsync();
        public Task<Tasks> GetTaskByIdAsync(int taskId);
        public Task<Tasks> AddTaskAsync(Tasks task);
        public Task<Tasks> UpdateTaskAsync(Tasks task);
        public Task DeleteTaskAsync(int taskId);
    }
}
