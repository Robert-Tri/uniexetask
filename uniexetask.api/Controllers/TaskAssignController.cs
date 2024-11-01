using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/task-assign")]
    [ApiController]
    public class TaskAssignController : ControllerBase
    {
        public ITaskService _taskService;
        public ITaskAssignService _taskAssignService;


        public TaskAssignController(ITaskService taskService, ITaskAssignService taskAssignService)
        {
            _taskService = taskService;
            _taskAssignService = taskAssignService;
        }

        [HttpGet("byStudent/{studentId}")]
        public async Task<IActionResult> GetTasksByStudent(string studentId)
        {
            var tasksList = await _taskAssignService.GetTaskAssignsByStudent(int.Parse(studentId));
            if (tasksList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<TaskAssign>> response = new ApiResponse<IEnumerable<TaskAssign>>();
            response.Data = tasksList;
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTaskAssign([FromBody] CreateTaskAssignModel taskAssignModel)
        {
            foreach (var studentId in taskAssignModel.StudentsId)
            {
                var createdTaskAssign = await _taskAssignService.CreateTaskAssign(new TaskAssign
                {
                    TaskId = taskAssignModel.TaskId,
                    StudentId = studentId,
                    AssignedDate = DateTime.Now
                });
                if (createdTaskAssign == null)
                {
                    return StatusCode(500, "Error creating task");
                }
            }
            return Ok();
        }
    }
}
