using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
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
            ApiResponse<IEnumerable<TaskDetail>> response = new ApiResponse<IEnumerable<TaskDetail>>();
            try
            {
                var taskDetailList = await _taskDetailService.GetTaskDetailListByTaskId(taskId);
                if (taskDetailList == null)
                {
                    throw new Exception("TaskDetailList not found");
                }
                response.Data = taskDetailList;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }

        }
    }
}
