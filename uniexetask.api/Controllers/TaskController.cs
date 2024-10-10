using Microsoft.AspNetCore.Mvc;
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


        public TaskController(ITaskService taskService, ITaskAssignService taskAssignService)
        {
            _taskService = taskService;
            _taskAssignService = taskAssignService;
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

    }
}
