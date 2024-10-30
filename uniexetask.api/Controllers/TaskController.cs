using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/task")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        public ITaskService _taskService;
        public ITaskAssignService _taskAssignService;
        public IStudentService _studentsService;
        public ITaskProgressService _taskProgressService;
        private readonly IMapper _mapper;


        public TaskController(ITaskService taskService, 
                            ITaskAssignService taskAssignService, 
                            IStudentService studentsService,
                            ITaskProgressService taskProgressService,
                            IMapper mapper)
        {
            _taskService = taskService;
            _taskAssignService = taskAssignService;
            _studentsService = studentsService;
            _taskProgressService = taskProgressService;
            _mapper = mapper;

        }

        [HttpGet("byProject/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(string projectId)
        {
            var tasksList = await _taskService.GetTasksByProject(int.Parse(projectId));
            if (tasksList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<core.Models.Task>> response = new ApiResponse<IEnumerable<core.Models.Task>>();
            response.Data = tasksList;
            return Ok(response);
        }

        [HttpGet("byStudent/{studentId}")]
        public async Task<IActionResult> GetTasksByStudent(string studentId)
        {
            var tasksList = await _taskService.GetTasksByStudent(int.Parse(studentId));
            if (tasksList == null)
            {
                return NotFound();
            }

            ApiResponse<IEnumerable<core.Models.Task>> response = new ApiResponse<IEnumerable<core.Models.Task>>();
            response.Data = tasksList;
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskModel task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var obj = _mapper.Map<core.Models.Task>(task);

            var createdTask = await _taskService.CreateTask(obj);
            if (!createdTask)
            {
                return StatusCode(500, "Error creating task");
            }

            return Ok(obj);
        }
    }
}
