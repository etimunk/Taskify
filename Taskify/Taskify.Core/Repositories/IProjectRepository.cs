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
        public Task<Project> GetByIDAsync(int id);
        public Task<Project> AddProject(Project project);
        public Task<Project> UpdateProjectAsync(Project project);
        public Task DeleteProjectAsync(int id);
        public Task saveAsync();
    }
}
