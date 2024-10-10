using Microsoft.AspNetCore.Mvc;
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
    }
}
