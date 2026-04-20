using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskify.Core.Entities;
using Taskify.Core.Repositories;

namespace Taskify.Data.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DataContext _context;
        public ProjectRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetALLAsync()
        {
            return await _context.projects.ToListAsync();
        }
        public async Task<List<Project>> GetProjectsByManagerIdAsync(int managerId)
        {
            return await _context.projects.Where(p=> p.ManagerId==managerId).ToListAsync();
        }

        public async Task<Project?> GetByIDAsync(int id)
        {
            return await _context.projects.FirstOrDefaultAsync(p => p.Id == id);
        }

        public void AddProject(Project project)
        {
            _context.projects.Add(project);
        }


        public async Task<Project> UpdateProjectAsync(Project project)
        {
            var pro = await GetByIDAsync(project.Id);
            if (pro != null)
            {
                pro.Name = project.Name;
                pro.Description = project.Description;
                pro.Status = project.Status;
                pro.StartDate = project.StartDate;
                pro.DueDate = project.DueDate;
                pro.Tasks = project.Tasks;
                pro.Manager = project.Manager;
                pro.ManagerId = project.ManagerId;
            }
            return project;
        }

        public async Task<Project> DeleteProjectAsync(int id)
        {
            var pro = await _context.projects.FirstOrDefaultAsync(p => p.Id == id);

            if (pro != null)
            {
                _context.projects.Remove(pro);
            }
            return pro; 
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
