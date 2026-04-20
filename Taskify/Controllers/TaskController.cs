
//using AutoMapper;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using Taskify.Core.DTOs;
//using Taskify.Core.Entities;
//using Taskify.Core.Servieces;
//using Taskify.Core.Servieces.Taskify.Core.Servieces;
//using Taskify.Models;
//using Taskify.Service;

//namespace Taskify.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class TaskController : ControllerBase
//    {
//        private readonly ITaskService _taskService;
//        private readonly IUserService _userService;
//        private readonly IProjectService _projectService;
//        private readonly IMapper _mapper;

//        public TaskController(ITaskService taskService, IMapper mapper, IUserService userService,IProjectService projectService)
//        {
//            _taskService = taskService;
//            _mapper = mapper;
//            _userService = userService;
//            _projectService = projectService;
//        }

//        // GET: api/<TaskController>
//        [HttpGet]
//        [Authorize(Roles = "headmanager")]
//        public async Task<ActionResult<IEnumerable<TasksDTO>>> Get()
//        {
//            var tasks = await _taskService.GetAllTasksAsync();
//            var tasksDto = _mapper.Map<IEnumerable<TasksDTO>>(tasks);
//            return Ok(tasksDto);
//        }
//        // GET api/<TaskController>/project/5
//        [HttpGet("project/{projectId}")]
//        [Authorize(Roles = "headmanager,manager")]
//        public async Task<ActionResult<IEnumerable<TasksDTO>>> GetTasksByProject(int projectId)
//        {
//            var project = await _projectService.GetByIDAsync(projectId);
//            if (project == null)
//            {
//                return NotFound("Project not found.");
//            }

//            var email = User.FindFirst(ClaimTypes.Email)?.Value;
//            var name = User.FindFirst(ClaimTypes.Name)?.Value;
//            var currentUser = await _userService.GetUsersByNameAndEmailAsync(name, email);

//            if (currentUser.Level.ToString() != "headmanager" && project.ManagerId != currentUser.Id)
//            {
//                return Forbid(); 
//            }

//            var tasks = await _taskService.GetTasksByProjectIdAsync(projectId);

//            var tasksDto = _mapper.Map<IEnumerable<TasksDTO>>(tasks);
//            return Ok(tasksDto);
//        }



//        // GET api/<TaskController>/project/5
//        [HttpGet("project/{userid}")]
//        [Authorize(Roles = "worker")]

//        public async Task<ActionResult<IEnumerable<TasksDTO>>> GetTasksByWorker(int userid)
//        {
//            var use = await _userService.GetUserByIDAsync(userid);
//            if (use == null)
//            {
//                return NotFound("User not found.");
//            }

//            var email = User.FindFirst(ClaimTypes.Email)?.Value;
//            var name = User.FindFirst(ClaimTypes.Name)?.Value;
//            var currentUser = await _userService.GetUsersByNameAndEmailAsync(name, email);

//            if ( currentUser.Id != userid)
//            {
//                var tasks = await _taskService.GetAllTasksByWorkerAsync(userid);

//                var tasksDto = _mapper.Map<IEnumerable<TasksDTO>>(tasks);
//                return Ok(tasksDto);
//            }
//            return Forbid();
//        }



//        // GET api/<TaskController>/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<TasksDTO>> Get(int id)
//        {
//            var task = await _taskService.GetTaskByIdAsync(id);
//            if (task == null)
//                return NotFound();

//            var email = User.FindFirst(ClaimTypes.Email)?.Value;
//            var name = User.FindFirst(ClaimTypes.Name)?.Value;
//            var user = await _userService.GetUsersByNameAndEmailAsync(name, email);
//            var project = await _projectService.GetByIDAsync(task.ProjectId);
//            if (project == null)
//                return NotFound(task);
//            if(user.Id==task.UserId|| user.Id==project.ManagerId||user.Role.ToString()== "headmanager")
//            {
//                var taskDto = _mapper.Map<TasksDTO>(task);
//                return Ok(taskDto);
//            }
//            return NoContent();
//        }

//        // POST api/<TaskController>
//        [HttpPost]
//        [Authorize(Roles = "manager")]

//        public async Task<ActionResult<TasksDTO>> Post([FromBody] TasksPostModel taskModdel)
//        {
//            // בדיקה אם המשימה כבר קיימת
//            var existing = await _taskService.GetTaskByIdAsync(taskModdel.Id);
//            if (existing != null)
//            {
//                return Conflict("Task with this ID already exists.");
//            }
//            var email = User.FindFirst(ClaimTypes.Email)?.Value;
//            var name = User.FindFirst(ClaimTypes.Name)?.Value;
//            var user = await _userService.GetUsersByNameAndEmailAsync(name, email);
//            var project = await _projectService.GetByIDAsync(taskModdel.ProjectId);

//            if (user != null) 
//                return NotFound(user);

//            if (user.Id != project.ManagerId)
//                return Forbid();

//            var taskEntity = _mapper.Map<Tasks>(taskModdel);
//            var addedTask =  _taskService.AddTaskAsync(taskEntity);

//            var resultDto = _mapper.Map<TasksDTO>(addedTask);
//            return CreatedAtAction(nameof(Get), new { id = resultDto.Id }, resultDto);
//        }

//        //// PUT api/<TaskController>/5
//        [HttpPut("{id}")]
//        [Authorize(Roles = "manager,worker")]

//        public async Task<ActionResult<TasksDTO>> Put(int id, [FromBody] TasksPostModel taskModdel)
//        {
//            var existingTask = await _taskService.GetTaskByIdAsync(id);
//            if (existingTask == null)
//            {
//                return NotFound();
//            }

//            var email = User.FindFirst(ClaimTypes.Email)?.Value;
//            var name = User.FindFirst(ClaimTypes.Name)?.Value;
//            var user = await _userService.GetUsersByNameAndEmailAsync(name, email);
//            var project = await _projectService.GetByIDAsync(taskModdel.ProjectId);

//            if (project != null)
//                return NoContent();
//            // עדכון הישות הקיימת בעזרת הנתונים מה-DTO
//            _mapper.Map(taskModdel, existingTask);
//            if (user.Id == existingTask.UserId)
//            {
//                var updatedTask = await _taskService.UpdateTaskWorkerAsync(existingTask);
//                var resultDto = _mapper.Map<TasksDTO>(updatedTask);
//                return Ok(resultDto);
//            }
//            else if (user.Level.ToString() == "manager" && user.Id == project.ManagerId)
//            {
//                var updatedTaskManager = await _taskService.UpdateTaskAsync(existingTask);
//                var resultDto = _mapper.Map<TasksDTO>(updatedTaskManager);
//                return Ok(resultDto);
//            }
//            return Forbid();

           
//        }

//        // DELETE api/<TaskController>/5
//        [HttpDelete("{id}")]
//        [Authorize(Roles = "manager")]
//        public async Task<ActionResult> Delete(int id)
//        {
//            var task = await _taskService.GetTaskByIdAsync(id);
//            if (task == null)
//            {
//                return NotFound();
//            }


//            var email = User.FindFirst(ClaimTypes.Email)?.Value;
//            var name = User.FindFirst(ClaimTypes.Name)?.Value;
//            var user = await _userService.GetUsersByNameAndEmailAsync(name, email);
//            var project= await _projectService.GetByIDAsync(task.ProjectId);
//            if (project == null) {
//                return NotFound();
//            }

//            if (user.Id == project.ManagerId)
//            {
//                await _taskService.DeleteTaskAsync(id);
//                return Ok();
//            }
//            return NoContent();
//        }
//    }
//}


using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Taskify.Core.DTOs;
using Taskify.Core.Entities;
using Taskify.Core.Servieces;
using Taskify.Core.Servieces.Taskify.Core.Servieces;
using Taskify.Models;
using Taskify.Service;

namespace Taskify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public TaskController(ITaskService taskService, IMapper mapper, IUserService userService, IProjectService projectService)
        {
            _taskService = taskService;
            _mapper = mapper;
            _userService = userService;
            _projectService = projectService;
        }

        // GET: api/<TaskController>
        [HttpGet]
        [Authorize(Roles = "headmanager")]
        public async Task<ActionResult<IEnumerable<TasksDTO>>> Get()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            var tasksDto = _mapper.Map<IEnumerable<TasksDTO>>(tasks);
            return Ok(tasksDto);
        }

        // GET api/<TaskController>/project/5
        [HttpGet("project/{projectId}")]
        [Authorize(Roles = "headmanager,manager")]
        public async Task<ActionResult<IEnumerable<TasksDTO>>> GetTasksByProject(int projectId)
        {
            var project = await _projectService.GetByIDAsync(projectId);
            if (project == null)
            {
                return NotFound("Project not found.");
            }

            // שליפה מעודכנת לפי מייל ותפקיד
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUser = await _userService.GetUsersByEmailAndRoleAsync(email, role);

            if (currentUser == null) return Unauthorized();

            if (currentUser.Level.ToString().ToLower() != "headmanager" && project.ManagerId != currentUser.Id)
            {
                return Forbid();
            }

            var tasks = await _taskService.GetTasksByProjectIdAsync(projectId);
            var tasksDto = _mapper.Map<IEnumerable<TasksDTO>>(tasks);
            return Ok(tasksDto);
        }

        // GET api/<TaskController>/worker/5
        [HttpGet("worker/{userid}")] // שיניתי ל-worker בנתיב כדי למנוע כפילות עם הפרויקט
        [Authorize(Roles = "worker")]
        public async Task<ActionResult<IEnumerable<TasksDTO>>> GetTasksByWorker(int userid)
        {
            var use = await _userService.GetUserByIDAsync(userid);
            if (use == null)
            {
                return NotFound("User not found.");
            }

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUser = await _userService.GetUsersByEmailAndRoleAsync(email, role);

            if (currentUser == null) return Unauthorized();

            // בדיקה שוורקר יכול לראות רק את המשימות של עצמו
            if (currentUser.Id == userid)
            {
                var tasks = await _taskService.GetAllTasksByWorkerAsync(userid);
                var tasksDto = _mapper.Map<IEnumerable<TasksDTO>>(tasks);
                return Ok(tasksDto);
            }
            return Forbid();
        }

        // GET api/<TaskController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TasksDTO>> Get(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var user = await _userService.GetUsersByEmailAndRoleAsync(email, role);
            
            if (user == null) return Unauthorized();

            var project = await _projectService.GetByIDAsync(task.ProjectId);
            if (project == null)
                return NotFound(task);

            if (user.Id == task.UserId || user.Id == project.ManagerId || role.ToLower() == "headmanager")
            {
                var taskDto = _mapper.Map<TasksDTO>(task);
                return Ok(taskDto);
            }
            return NoContent();
        }

        // POST api/<TaskController>
        [HttpPost]
        [Authorize(Roles = "manager")]
       
        public async Task<ActionResult<TasksDTO>> Post([FromBody] TasksPostModel taskModdel)
        {
            // 1. בדיקה אם המשימה כבר קיימת
            var existing = await _taskService.GetTaskByIdAsync(taskModdel.Id);
            if (existing != null)
            {
                return Conflict("Task with this ID already exists.");
            }
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var user = await _userService.GetUsersByEmailAndRoleAsync(email, role);

            if (user == null)
                return Unauthorized("User not found in system.");
            var project = await _projectService.GetByIDAsync(taskModdel.ProjectId);
            if (project == null)
                return NotFound("Project not found.");

            if (user.Id != project.ManagerId)
                return Forbid();
            var taskEntity = _mapper.Map<Tasks>(taskModdel);
            await _taskService.AddTaskAsync(taskEntity);
            var resultDto = _mapper.Map<TasksDTO>(taskEntity);
            return CreatedAtAction(nameof(Get), new { id = resultDto.Id }, resultDto);
        }

        // PUT api/<TaskController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "manager,worker")]
        public async Task<ActionResult<TasksDTO>> Put(int id, [FromBody] TasksPostModel taskModdel)
        {
            var existingTask = await _taskService.GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var user = await _userService.GetUsersByEmailAndRoleAsync(email, role);
            
            if (user == null) return Unauthorized();

            var project = await _projectService.GetByIDAsync(taskModdel.ProjectId);

            _mapper.Map(taskModdel, existingTask);
            
            if (user.Id == existingTask.UserId)
            {
                var updatedTask = await _taskService.UpdateTaskWorkerAsync(existingTask);
                var resultDto = _mapper.Map<TasksDTO>(updatedTask);
                return Ok(resultDto);
            }
            else if (role.ToLower() == "manager" && project != null && user.Id == project.ManagerId)
            {
                var updatedTaskManager = await _taskService.UpdateTaskAsync(existingTask);
                var resultDto = _mapper.Map<TasksDTO>(updatedTaskManager);
                return Ok(resultDto);
            }
            return Forbid();
        }

        // DELETE api/<TaskController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Delete(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var user = await _userService.GetUsersByEmailAndRoleAsync(email, role);
            
            if (user == null) return Unauthorized();

            var project = await _projectService.GetByIDAsync(task.ProjectId);
            if (project == null)
            {
                return NotFound();
            }

            if (user.Id == project.ManagerId)
            {
                await _taskService.DeleteTaskAsync(id);
                return Ok();
            }
            return Forbid();
        }
    }
}