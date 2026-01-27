using System.Collections.Generic;
using System.Threading.Tasks;
using Taskify.Core.Entities;
using Taskify.Core.Repositories;
using Taskify.Core.Servieces;

namespace Taskify.Service
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<List<Tasks>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllTasksAsync();
        }

        public async Task<Tasks> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetTaskByIdAsync(id);
        }

        public async Task<Tasks> AddTaskAsync(Tasks task)
        {
            var t = await _taskRepository.AddTaskAsync(task);
            await _taskRepository.SaveAsync();
            return t;
        }

        public async Task<Tasks> UpdateTaskAsync(Tasks task)
        {
            var updatedTask = await _taskRepository.UpdateTaskAsync(task);
            if (updatedTask != null)
            {
                await _taskRepository.SaveAsync();
            }
            return updatedTask;
        }

        public async Task DeleteTaskAsync(int id)
        {
            await _taskRepository.DeleteTaskAsync(id);
            await _taskRepository.SaveAsync();
        }
    }
}
