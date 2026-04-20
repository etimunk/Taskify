//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using Taskify.Core.DTOs;
//using Taskify.Core.Entities;
//using Taskify.Core.Servieces;

//namespace Taskify.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserController : ControllerBase
//    {
//        private readonly IUserService _userService;
//        private readonly IMapper _mapper;

//        public UserController(IUserService userService, IMapper mapper)
//        {
//            _userService = userService;
//            _mapper = mapper;
//        }

//        GET: api/<UserController>
//        [HttpGet]
//        public IActionResult Get()
//        {
//            var users = _userService.GetUsers();
//            var usersDTO = _mapper.Map<List<UserDTO>>(users); // המרה מ-User ל-UserDTO
//            return Ok(usersDTO);
//        }

//        GET api/<UserController>/5
//        [HttpGet("{id}")]
//        public ActionResult Get(int id)
//        {
//            var user = _userService.GetUserByID(id);
//            if (user == null)
//                return NotFound();

//            var userDto = _mapper.Map<UserDTO>(user); // המרה מ-User ל-UserDTO
//            return Ok(userDto);
//        }

//        POST api/<UserController>
//        [HttpPost]
//        public ActionResult Post([FromBody] UserDTO value)
//        {
//            var existingUser = _userService.GetUserByID(value.Id);
//            if (existingUser != null)
//            {
//                return Conflict(); // אם כבר קיים
//            }

//            var user = _mapper.Map<User>(value); // המרה מ-UserDTO ל-User
//            _userService.AddUser(user); // הוספת המשתמש
//            var createdUserDTO = _mapper.Map<UserDTO>(user); // המרה מ-User ל-UserDTO
//            return CreatedAtAction(nameof(Get), new { id = createdUserDTO.Id }, createdUserDTO);
//        }

//        PUT api/<UserController>/5
//        [HttpPut("{id}")]
//        public ActionResult Put(int id, [FromBody] UserDTO value)
//        {
//            var existingUser = _userService.GetUserByID(id);
//            if (existingUser == null)
//            {
//                return NotFound(); // אם לא נמצא
//            }

//            ממפה את הנתונים החדשים ל - User
//            var userToUpdate = _mapper.Map<User>(value);
//            userToUpdate.Id = id; // לוודא שה-Id נשאר אותו דבר

//            _userService.UpdateUser(userToUpdate); // מעדכן את המשתמש
//            var updatedUserDTO = _mapper.Map<UserDTO>(userToUpdate); // ממפה ל-UserDTO
//            return Ok(updatedUserDTO);
//        }

//        DELETE api/<UserController>/5
//        [HttpDelete("{id}")]
//        public ActionResult Delete(int id)
//        {
//            var user = _userService.GetUserByID(id);
//            if (user == null)
//            {
//                return NotFound(); // אם לא נמצא
//            }

//            _userService.DeleteUser(id); // מוחק את המשתמש
//            return NoContent(); // אין תוכן אחרי מחיקה
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

namespace Taskify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
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