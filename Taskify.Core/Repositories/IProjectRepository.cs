using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskify.Core.Entities;


namespace Taskify.Core.Repositories
{
    public interface IProjectRepository
    {
        public Task<List<Project>> GetALLAsync();
        public Task<List<Project>> GetProjectsByManagerIdAsync(int managerId);
        public Task<Project> GetByIDAsync(int id);
        public void AddProject(Project project);
        public Task<Project> UpdateProjectAsync(Project project);
        public Task<Project> DeleteProjectAsync(int taskId);
        public Task SaveAsync();
    }
}
