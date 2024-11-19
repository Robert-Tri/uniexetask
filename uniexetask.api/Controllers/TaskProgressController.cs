using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/task-progress")]
    [ApiController]
    public class TaskProgressController : ControllerBase
    {
        private readonly ITaskProgressService _taskProgressService;
        private readonly ITaskDetailService _taskDetailService;
        private readonly ITaskService _taskService;

        public TaskProgressController(ITaskProgressService taskProgressService, ITaskDetailService taskDetailService, ITaskService taskService)
        {
            _taskProgressService = taskProgressService;
            _taskDetailService = taskDetailService;
            _taskService = taskService;
        }

        [HttpPut("loadTaskProgressByTaskId/{taskId}")]
        public async Task<IActionResult> LoadUpdateTaskProgressByTaskId(int taskId)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                var loadProgress = await _taskProgressService.LoadProgressUpdateTaskProgressByTaskId(taskId);
                if (!loadProgress)
                {
                    throw new Exception("Error loading TaskProgress");
                }
                response.Data = loadProgress;
                response.Success = true;
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
