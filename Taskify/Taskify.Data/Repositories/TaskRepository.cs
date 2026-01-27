using Microsoft.EntityFrameworkCore;
using Taskify.Core.Entities;
using Taskify.Core.Repositories;

namespace Taskify.Data.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly DataContext _context;
        public TaskRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Tasks>> GetAllTasksAsync()
        {
            return await _context.tasks.Include(t => t.Project).Include(t => t.User).ToListAsync();
        }

        public async Task<Tasks> GetTaskByIdAsync(int id)
        {
            return await _context.tasks.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tasks> AddTaskAsync(Tasks task)
        {
            await _context.tasks.AddAsync(task);
            return task;
        }

        public async Task<Tasks> UpdateTaskAsync(Tasks task)
        {
            var tas = await _context.tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
            if (tas != null)
            {
                tas.Name = task.Name;
                tas.Description = task.Description;
                tas.Type = task.Type;
                tas.Priority = task.Priority;
                tas.Project = task.Project;
                tas.User = task.User;
                tas.ProjectId = task.ProjectId;
                tas.UserId = task.UserId;
            }
            return tas;
        }

        public async Task DeleteTaskAsync(int id)
        {
            var tas = await _context.tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (tas != null)
            {
                _context.tasks.Remove(tas);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}