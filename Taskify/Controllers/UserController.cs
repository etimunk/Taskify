





using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Taskify.Core.DTOs;
using Taskify.Core.Entities;
using Taskify.Core.Servieces;

namespace Taskify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IProjectService projectService, ITaskService taskService, IMapper mapper)
        {
            _userService = userService;
            _projectService = projectService;
            _taskService = taskService;
            _mapper = mapper;
        }

        // GET: api/<UserController>
        [HttpGet]
        [Authorize(Roles = "headmanager")]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.GetUsersAsync();
            var usersDTO = _mapper.Map<List<UserDTO>>(users);
            return Ok(usersDTO);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult> Get(int id)
        {
            // שליפה מהטוקן
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUser = await _userService.GetUsersByEmailAndRoleAsync(email, role);

            // בדיקה: אם המשתמש הוא worker, הוא יכול לראות רק את עצמו
            if (currentUser != null && currentUser.Level.ToString() == "worker" && currentUser.Id != id)
            {
                return Forbid();
            }

            var user = await _userService.GetUserByIDAsync(id);
            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        }

        // GET api/<UserController>/manager/5/workers
        [HttpGet("manager/{managerId}/workers")]
        [Authorize(Roles = "manager,headmanager")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetWorkersByManager(int managerId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUser = await _userService.GetUsersByEmailAndRoleAsync(email, role);

            if (currentUser == null) return Unauthorized();

            // A regular manager can only query within their own scope.
            if (string.Equals(role, "manager", StringComparison.OrdinalIgnoreCase) && currentUser.Id != managerId)
            {
                return Forbid();
            }

            var projects = await _projectService.GetProjectsByManagerIdAsync(managerId);
            if (projects == null || projects.Count == 0)
            {
                return Ok(new List<UserDTO>());
            }

            var workerIds = new HashSet<int>();
            foreach (var project in projects)
            {
                var tasks = await _taskService.GetTasksByProjectIdAsync(project.Id);
                foreach (var task in tasks)
                {
                    workerIds.Add(task.UserId);
                }
            }

            var allUsers = await _userService.GetUsersAsync();
            var workers = allUsers
                .Where(u =>
                    workerIds.Contains(u.Id) &&
                    string.Equals(u.Level.ToString(), "worker", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(_mapper.Map<List<UserDTO>>(workers));
        }

        // GET api/<UserController>/workers
        [HttpGet("workers")]
        [Authorize(Roles = "manager,headmanager")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllWorkers()
        {
            var allUsers = await _userService.GetUsersAsync();
            var workers = allUsers
                .Where(u => string.Equals(u.Level.ToString(), "worker", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(_mapper.Map<List<UserDTO>>(workers));
        }

        // POST api/<UserController>
        [HttpPost]
        [Authorize(Roles = "headmanager")]
        public async Task<ActionResult> Post([FromBody] UserDTO value)
        {
            var existingUser = await _userService.GetUserByIDAsync(value.Id);
            if (existingUser != null)
            {
                return Conflict();
            }

            var user = _mapper.Map<User>(value);
            await _userService.AddUserAsync(user);
            var createdUserDTO = _mapper.Map<UserDTO>(user);
            return CreatedAtAction(nameof(Get), new { id = createdUserDTO.Id }, createdUserDTO);
        }

 

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Put(int id, [FromBody] UserDTO value)
        {
            // 1. שליפת המשתמש המחובר כרגע מהטוקן
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUser = await _userService.GetUsersByEmailAndRoleAsync(email, role);

            if (currentUser == null) return Unauthorized();

            // 2. בדיקה: האם המשתמש מנסה לעדכן את עצמו?
            if (currentUser.Id == id)
            {
                var userToUpdate = _mapper.Map<User>(value);
                userToUpdate.Id = id;
                await _userService.UpdateUserAsync(userToUpdate); // עדכון פרטים אישיים
            }

            // 3. בדיקה: האם המחובר הוא headmanager (שיכול לעדכן תפקיד ודרגה לאחרים)?
            // שימי לב: המנהל יכול לעדכן אחרים, או את עצמו (בנוסף לפרטים האישיים)
            if (role == "headmanager")
            {
                var userToUpdate = _mapper.Map<User>(value);
                userToUpdate.Id = id;
                await _userService.UpdateUserForHeadManagerAsync(userToUpdate); // עדכון תפקיד ודרגה
            }
            else if (currentUser.Id != id)
            {
                // אם הוא לא המנהל והוא לא מעדכן את עצמו - אין לו הרשאה
                return Forbid();
            }

            // שליפה מחדש של המשתמש כדי להחזיר את הנתונים המעודכנים באמת מה-DB
            var updatedUser = await _userService.GetUserByIDAsync(id);
            return Ok(_mapper.Map<UserDTO>(updatedUser));
        }


        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "headmanager")]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await _userService.GetUserByIDAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}