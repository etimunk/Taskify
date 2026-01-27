using Microsoft.EntityFrameworkCore;
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
            
            return await _context.projects.Include(p => p.Manager).Include(p => p.Tasks).ToListAsync();
        }

        public async Task<Project> GetByIDAsync(int id)
        {
            return await _context.projects.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Project> AddProject(Project project)
        {
            if (project.Manager != null && project.Manager.Id > 0)
            {
                _context.Entry(project.Manager).State = EntityState.Unchanged;
            }

            await _context.projects.AddAsync(project);
            return project;
        }

        public async Task<Project> UpdateProjectAsync(Project project)
        {
            var pro = await _context.projects.FirstOrDefaultAsync(p => p.Id == project.Id);
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
            return pro;
        }

        public async Task DeleteProjectAsync(int id)
        {
            var pro = await _context.projects.FirstOrDefaultAsync(p => p.Id == id);
            if (pro != null)
            {
                _context.projects.Remove(pro);
            }
        }
        public async Task saveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
