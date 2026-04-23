

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ProjectController(IProjectService projectService, IMapper mapper, IUserService userService)
        {
            _projectService = projectService;
            _mapper = mapper;
            _userService = userService;
        }

        // GET: api/<ProjectController>
        [HttpGet]
        [Authorize(Roles = "headmanager")]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> Get()
        {
            var pList = await _projectService.GetProjectsAsync();
            var pDTOList = _mapper.Map<IEnumerable<ProjectDTO>>(pList);
            return Ok(pDTOList);
        }

        // GET api/<ProjectController>/manager/5
        [HttpGet("manager/{managerId}")]
        [Authorize(Roles = "manager,headmanager")] // הוספתי headmanager למקרה שירצה לראות פרויקטים של מנהל ספציפי
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetByManager(int managerId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // שליפה לפי מייל ותפקיד כפי שסיכמנו
            var currentUser = await _userService.GetUsersByEmailAndRoleAsync(email, role);

            if (currentUser == null) return Unauthorized();

            // אם הוא מנהל רגיל, הוא יכול לראות רק את הפרויקטים שלו
            if (role.ToLower() == "manager" && currentUser.Id != managerId)
            {
                return Forbid();
            }

            var projects = await _projectService.GetProjectsByManagerIdAsync(managerId);

            if (projects == null || !projects.Any())
            {
                return Ok(new List<ProjectDTO>());
            }

            var projectDtos = _mapper.Map<IEnumerable<ProjectDTO>>(projects);
            return Ok(projectDtos);
        }

        // GET api/<ProjectController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "headmanager,manager")]
        public async Task<ActionResult<ProjectDTO>> Get(int id)
        {
            var project = await _projectService.GetByIDAsync(id);
            if (project == null) return NotFound();

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var user = await _userService.GetUsersByEmailAndRoleAsync(email, role);

            if (user == null) return Unauthorized();

            // בדיקת הרשאה: או שהוא המנהל של הפרויקט או שהוא מנהל על
            if (user.Id == project.ManagerId || role.ToLower() == "headmanager")
            {
                var projectDto = _mapper.Map<ProjectDTO>(project);
                return Ok(projectDto);
            }

            return Forbid();
        }

        // POST api/<ProjectController>
        [HttpPost]
        [Authorize(Roles = "headmanager")]
        public async Task<ActionResult<ProjectDTO>> Post([FromBody] ProjectPostModel projectModel)
        {
            var projectEntity = _mapper.Map<Project>(projectModel);

            // אם לא נשלח ManagerId, נשייך אותו למנהל הנוכחי שיצר את הפרויקט
            if (projectEntity.ManagerId <= 0)
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var user = await _userService.GetUsersByEmailAndRoleAsync(email, role);

                if (user != null)
                {
                    projectEntity.ManagerId = user.Id;
                }
                else
                {
                    // ברירת מחדל למקרה חירום (המנהל הראשון ב-DB)
                    projectEntity.ManagerId = 1;
                }
            }

            await _projectService.AddProjectAsync(projectEntity);

            var resultDto = _mapper.Map<ProjectDTO>(projectEntity);
            return CreatedAtAction(nameof(Get), new { id = resultDto.Id }, resultDto);
        }

        // PUT api/<ProjectController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "headmanager,manager")]
        public async Task<ActionResult<ProjectDTO>> Put(int id, [FromBody] ProjectPostModel projectModel)
        {
            var existingProject = await _projectService.GetByIDAsync(id);
            if (existingProject == null) return NotFound();

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var user = await _userService.GetUsersByEmailAndRoleAsync(email, role);

            if (user == null) return Forbid();

            // בדיקת הרשאה: רק המנהל האחראי על הפרויקט או ה-HeadManager יכולים לעדכן
            if (existingProject.ManagerId != user.Id && role.ToLower() != "headmanager")
            {
                return Forbid();
            }

            _mapper.Map(projectModel, existingProject);
            var updatedProject = await _projectService.UpdateProjectAsync(existingProject);
            var resultDto = _mapper.Map<ProjectDTO>(updatedProject);

            return Ok(resultDto);
        }

        // DELETE api/<ProjectController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "headmanager")]
        public async Task<ActionResult> Delete(int id)
        {
            var project = await _projectService.GetByIDAsync(id);
            if (project == null) return NotFound();

            await _projectService.DeleteProjectAsync(id);
            return NoContent();
        }
    }
}
