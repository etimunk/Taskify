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
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public TaskController(ITaskService taskService, IMapper mapper)
        {
            _taskService = taskService;
            _mapper = mapper;
        }

        // GET: api/<TaskController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDTO>>> Get()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            var tasksDto = _mapper.Map<IEnumerable<TaskDTO>>(tasks);
            return Ok(tasksDto);
        }

        // GET api/<TaskController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDTO>> Get(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            var taskDto = _mapper.Map<TaskDTO>(task);
            return Ok(taskDto);
        }

        // POST api/<TaskController>
        [HttpPost]
        public async Task<ActionResult<TaskDTO>> Post([FromBody] TaskPostModel taskModdel)
        {
            // בדיקה אם המשימה כבר קיימת
            var existing = await _taskService.GetTaskByIdAsync(taskModdel.Id);
            if (existing != null)
            {
                return Conflict("Task with this ID already exists.");
            }

            var taskEntity = _mapper.Map<Tasks>(taskModdel);
            var addedTask = await _taskService.AddTaskAsync(taskEntity);

            var resultDto = _mapper.Map<TaskDTO>(addedTask);
            return CreatedAtAction(nameof(Get), new { id = resultDto.Id }, resultDto);
        }

        // PUT api/<TaskController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDTO>> Put(int id, [FromBody] TaskPostModel taskModdel)
        {
            var existingTask = await _taskService.GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            // עדכון הישות הקיימת בעזרת הנתונים מה-DTO
            _mapper.Map(taskModdel, existingTask);

            var updatedTask = await _taskService.UpdateTaskAsync(existingTask);
            var resultDto = _mapper.Map<TaskDTO>(updatedTask);

            return Ok(resultDto);
        }

        // DELETE api/<TaskController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }
    }
}