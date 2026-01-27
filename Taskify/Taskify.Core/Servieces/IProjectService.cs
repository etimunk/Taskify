using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskify.Core.Entities;
using Taskify.Core.Repositories;

namespace Taskify.Core.Servieces
{
    public interface IProjectService
    {
        public Task<List<Project>> GetALLAsync();
        public Task<Project> GetByIDAsync(int id);
        public Task<Project> AddProjectAsync(Project project);
        public Task<Project> UpdateProjectAsync(Project project);
        public Task DeleteProjectAsync(int id);
    }
}
