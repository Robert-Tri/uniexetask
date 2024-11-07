using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/task-detail")]
    [ApiController]
    public class TaskDetailController : ControllerBase
    {
        private readonly ITaskDetailService _taskDetailService;

        public TaskDetailController(ITaskDetailService taskDetailService)
        {
            _taskDetailService = taskDetailService;
        }

        [HttpGet("byTask/{taskId}")]
        public async Task<IActionResult> GetTaskDetailListByTaskId(int taskId)
        {
            var taskDetailList = await _taskDetailService.GetTaskDetailListByTaskId(taskId);
            if (taskDetailList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<TaskDetail>> response = new ApiResponse<IEnumerable<TaskDetail>>();
            response.Data = taskDetailList;
            return Ok(response);
        }
    }
}
