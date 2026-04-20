using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Taskify.Core.Entities;
using Taskify.Core.Repositories;
using Taskify.Core.Servieces;
using Taskify.Core.Servieces.Taskify.Core.Servieces;

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
        public async Task<List<Tasks>> GetTasksByProjectIdAsync(int projectId)
        {
            return await _taskRepository.GetTasksByProjectIdAsync(projectId);
        }
        public async Task<List<Tasks>> GetAllTasksByWorkerAsync(int userId)
        {
            return await _taskRepository.GetAllTasksByWorkerAsync(userId);
        }
        public async Task<Tasks> GetTaskByIdAsync(int id)
        {
            return  await _taskRepository.GetTaskByIdAsync(id);
        }
        public  async Task AddTaskAsync(Tasks task)
        {
            _taskRepository.AddTaskAsync(task);
             await _taskRepository.SaveAsync();
        }
        public async Task<Tasks> UpdateTaskAsync(Tasks task)
        {
           var t= await  _taskRepository.UpdateTaskAsync(task);
            await _taskRepository.SaveAsync();
            return t;

        }
        public async Task<Tasks> UpdateTaskWorkerAsync(Tasks task)
        {
            var t = await _taskRepository.UpdateTaskWorkerAsync(task);
            await _taskRepository.SaveAsync();
            return t;
        }


        public async Task DeleteTaskAsync(int id)
        {
            await _taskRepository.DeleteTaskAsync(id);
           await  _taskRepository.SaveAsync();
        }

    }
}
