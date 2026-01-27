using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Taskify.Core.DTOs;
using Taskify.Core.Entities;
using Taskify.Core.Servieces;
using Taskify.Models;

namespace Taskify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public ProjectController(IProjectService projectService, IMapper mapper)
        {
            _projectService = projectService;
            _mapper = mapper;
        }

        // GET: api/<ProjectController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> Get()
        {
            var pList = await _projectService.GetALLAsync();
            var pDTOList = _mapper.Map<IEnumerable<ProjectDTO>>(pList);
            return Ok(pDTOList);
        }

        // GET api/<ProjectController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDTO>> Get(int id)
        {
            var project = await _projectService.GetByIDAsync(id);
            if (project == null)
                return NotFound();

            var projectDto = _mapper.Map<ProjectDTO>(project);
            return Ok(projectDto);
        }

        // POST api/<ProjectController>
        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> Post([FromBody] ProjectPostModel projectModel)
        {
            var projectEntity = _mapper.Map<Project>(projectModel);

            // בדיקה אם הפרויקט כבר קיים (לפי לוגיקה עסקית)
            var existing = await _projectService.GetByIDAsync(projectEntity.Id);
            if (existing != null)
            {
                return Conflict("Project already exists");
            }

            var addedProject = await _projectService.AddProjectAsync(projectEntity);

            var resultDto = _mapper.Map<ProjectDTO>(addedProject);
            return CreatedAtAction(nameof(Get), new { id = resultDto.Id }, resultDto);
        }

        // PUT api/<ProjectController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectDTO>> Put(int id, [FromBody] ProjectPostModel projectModel)
        {
            var existingProject = await _projectService.GetByIDAsync(id);
            if (existingProject == null)
            {
                return NotFound();
            }

            _mapper.Map(projectModel, existingProject);

            var updatedProject = await _projectService.UpdateProjectAsync(existingProject);
            var resultDto = _mapper.Map<ProjectDTO>(updatedProject);

            return Ok(resultDto);
        }

        // DELETE api/<ProjectController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var project = await _projectService.GetByIDAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            await _projectService.DeleteProjectAsync(id);
            return NoContent();
        }
    }
}
