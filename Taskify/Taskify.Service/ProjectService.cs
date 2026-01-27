
using System;
using System.Collections.Generic;
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

        public async Task<List<Project>> GetALLAsync()
        {
            return await _projectRepository.GetALLAsync();
        }

        public async Task<Project> GetByIDAsync(int id)
        {
            return await _projectRepository.GetByIDAsync(id);
        }

        public async Task<Project> AddProjectAsync(Project project)
        {
            var p = await _projectRepository.AddProject(project);
            await _projectRepository.saveAsync();
            return p;
        }

        public async Task<Project> UpdateProjectAsync(Project project)
        {
            var updatedProject = await _projectRepository.UpdateProjectAsync(project);

            if (updatedProject != null)
            {
                await _projectRepository.saveAsync();
            }

            return updatedProject;
        }

        public async Task DeleteProjectAsync(int id)
        {
            await _projectRepository.DeleteProjectAsync(id);
            await _projectRepository.saveAsync();
        }
    }
}