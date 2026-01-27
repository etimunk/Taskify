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
        public async Task<ActionResult<IEnumerable<UserDTO>>> Get()
        {
            var users = await _userService.GetUsersAsync();
            var usersDto = _mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(usersDto);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            var user = await _userService.GetUserByIDAsync(id);
            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Post([FromBody] UserPostModel userModel)
        {
            // בדיקה אם המשתמש קיים כבר לפי מזהה או אימייל (לוגיקה עסקית)
            var existingUser = await _userService.GetUserByIDAsync(userModel.Id);
            if (existingUser != null)
            {
                return Conflict("User already exists.");
            }

            var userEntity = _mapper.Map<User>(userModel);
            var addedUser = await _userService.AddUserAsync(userEntity);

            var resultDto = _mapper.Map<UserDTO>(addedUser);
            return CreatedAtAction(nameof(Get), new { id = resultDto.Id }, resultDto);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDTO>> Put(int id, [FromBody] UserPostModel userModel)
        {
            var existingUser = await _userService.GetUserByIDAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            _mapper.Map(userModel, existingUser);

            var updatedUser = await _userService.UpdateUserAsync(existingUser);
            var resultDto = _mapper.Map<UserDTO>(updatedUser);

            return Ok(resultDto);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
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