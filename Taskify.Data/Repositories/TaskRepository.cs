using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return await _context.tasks.ToListAsync();
        }
        public async Task<List<Tasks>> GetTasksByProjectIdAsync(int projectId)
        {
            return await _context.tasks.Where(t=> t.ProjectId == projectId).ToListAsync();
        }
        public async Task<List<Tasks>> GetAllTasksByWorkerAsync(int userId)
        {
            return await _context.tasks.Where(t=> t.UserId == userId).ToListAsync();
        }
        public  async Task<Tasks> GetTaskByIdAsync(int id)
        {
            return await _context.tasks.FirstOrDefaultAsync(t => t.Id == id);
        }
        public Tasks AddTaskAsync(Tasks task)
        {
            _context.tasks.Add(task);
                return task;
        }
        public async Task<Tasks> UpdateTaskAsync(Tasks task)
        {
            var tas = await GetTaskByIdAsync(task.Id);
            if (tas != null)
            {
                tas.Name = task.Name;
                tas.Description = task.Description;
                tas.Type = task.Type;
                tas.Priority = task.Priority;
                tas.ProjectId = task.ProjectId; 
                tas.UserId = task.UserId;      
            }

            return tas;
        }
        public async Task<Tasks> UpdateTaskWorkerAsync(Tasks task)
        {
            var tas = await GetTaskByIdAsync(task.Id);
            if (tas != null)
            {
                if(tas.Type!="done")
                tas.Type = task.Type;

            }
            return tas;
        }

        // הוספנו async כדי שנוכל להשתמש ב-await
        public async Task<Tasks> DeleteTaskAsync(int id)
        {
            var tas = await _context.tasks.FindAsync(id);
            if (tas != null)
            { 
                _context.tasks.Remove(tas);
            }

            return tas; 
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}



