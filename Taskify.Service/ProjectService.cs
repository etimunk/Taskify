using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskify.Core.Entities;
using Taskify.Core.Repositories;
using Taskify.Core.Servieces;



namespace Taskify.Service
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _projectRepository.GetALLAsync();
        }
        public async Task<List<Project>> GetProjectsByManagerIdAsync(int managerId)
        {
            return await _projectRepository.GetProjectsByManagerIdAsync(managerId);
        }
        public async Task<Project> GetByIDAsync(int id)
        {
            return  await _projectRepository.GetByIDAsync(id);
        }
        public async Task AddProjectAsync(Project project)
        {
            _projectRepository.AddProject(project); // לא async
            await _projectRepository.SaveAsync();
        }

        public async Task<Project> UpdateProjectAsync(Project project)
        {
            var p=await _projectRepository.UpdateProjectAsync(project);
            await _projectRepository.SaveAsync();
            return p;
        }
        public async Task DeleteProjectAsync(int id)
        {
            await _projectRepository.DeleteProjectAsync(id);
            await _projectRepository.SaveAsync();
        }

    }
}
