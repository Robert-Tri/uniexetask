using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;
using uniexetask.core.Models.Enums;
using Microsoft.AspNetCore.SignalR;
using uniexetask.services.Hubs;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/task-detail")]
    [ApiController]
    public class TaskDetailController : ControllerBase
    {
        private readonly ITaskDetailService _taskDetailService;
        private readonly ITaskService _taskService;
        private readonly ITaskProgressService _taskProgressService;
        private readonly IEmailService _emailService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IProjectProgressService _projectProgressService;
        private readonly IProjectService _projectService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;

        public TaskDetailController(ITaskDetailService taskDetailService,
            ITaskService taskService,
            ITaskProgressService taskProgressService,
            IEmailService emailService,
            IGroupMemberService groupMemberService,
            IProjectProgressService projectProgressService,
            IProjectService projectService,
            IHubContext<NotificationHub> hubContext,
            INotificationService notificationService)
        {
            _taskDetailService = taskDetailService;
            _taskService = taskService;
            _taskProgressService = taskProgressService;
            _emailService = emailService;
            _groupMemberService = groupMemberService;
            _projectProgressService = projectProgressService;
            _projectService = projectService;
            _hubContext = hubContext;
            _notificationService = notificationService;
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
                // Lọc bỏ những task detail có isDelete == true
                var filteredTaskDetailList = taskDetailList.Where(taskDetail => !taskDetail.IsDeleted).ToList();

                response.Data = filteredTaskDetailList;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }

        }

        [HttpDelete("{taskDetailId}")]
        public async Task<IActionResult> DeleteTaskDetail(int taskDetailId)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                // Kiểm tra giá trị taskId hợp lệ
                if (taskDetailId <= 0)
                {
                    response.Success = false;
                    response.ErrorMessage = "Invalid task detail ID.";
                    return BadRequest(response);
                }

                // Gọi phương thức DeleteTask từ TaskService
                var result = await _taskDetailService.DeleteTaskDetail(taskDetailId);
                if (result)
                {
                    response.Success = true;
                    response.Data = "Task Detail deleted successfully.";
                    return Ok(response);
                }
                else
                {
                    response.Success = false;
                    response.ErrorMessage = "Task Detail not found.";
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPut("updateStatus")]
        public async Task<IActionResult> UpdateStatusTaskDetail([FromBody] UpdateStatusTaskDetailModel model)
        {
            ApiResponse<TaskDetail> response = new ApiResponse<TaskDetail>();
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (!ModelState.IsValid)
                {
                    throw new Exception("Invalid request model");
                }

                // Lấy task từ database theo taskId
                var existingTaskDetail = await _taskDetailService.GetTaskDetailById(model.TaskDetailId);
                if (existingTaskDetail == null)
                {
                    response.Success = false;
                    response.ErrorMessage = "Task not found.";
                    return NotFound(response);
                }

                // Cập nhật trạng thái của task
                existingTaskDetail.IsCompleted = model.IsCompleted;

                // Lưu thay đổi vào cơ sở dữ liệu
                var updatedTaskDetail = await _taskDetailService.UpdateTaskDetails(existingTaskDetail);
                if (!updatedTaskDetail)
                {
                    throw new Exception("Error updating task detail status");
                }

                // Kiểm tra cập nhật Progress và Status
                var loadProgressTaskProgress = await _taskProgressService.LoadProgressUpdateTaskProgressByTaskId(existingTaskDetail.TaskId);
                if (loadProgressTaskProgress)
                {
                    var loadStatusTask = await _taskService.LoadStatusCompletedTaskByTaskId(existingTaskDetail.TaskId);
                    if (loadStatusTask)
                    {
                        var task = await _taskService.GetTaskById(existingTaskDetail.TaskId);
                        var projectId = task.ProjectId;
                        var loadProgressProject = await _projectProgressService.LoadProgressUpdateProjectProgressByProjectId(projectId);
                        if (!loadProgressProject)
                        {
                            throw new Exception("Error updating task detail status");
                        }
                    }
                    var checkComplete = await _taskService.GetTaskById(existingTaskDetail.TaskId);
                    if(checkComplete.Status == nameof(TasksStatus.Completed))
                    {
                        var taskEmail = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            padding: 20px;
            background-color: #ffffff; 
        }}
        h2 {{
            color: #333;
        }}
        p {{
            margin: 0 0 10px;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
        }}
        table, th, td {{
            border: 1px solid #ddd;
        }}
        th, td {{
            padding: 8px;
            text-align: left;
        }}
        th {{
            background-color: #f4f4f4;
        }}
    </style>
</head>
<body>
    <h2>Dear EXE Students,</h2>

    <p>A task in your project has been COMPLETED!</p>

    <p><strong>Task Information:</strong></p>
    <p><strong>Name:</strong> {checkComplete.TaskName}</p>
    <p><strong>Start Date:</strong> {checkComplete.StartDate.ToString("MMMM dd, yyyy")}</p>
    <p><strong>Deadline:</strong> {checkComplete.EndDate.ToString("MMMM dd, yyyy")}</p>
    <p><strong>Description:</strong> {checkComplete.Description}</p>
    

    <p>This is an automated email. Please do not reply to this email.</p>
    <p>Looking forward to your participation.</p>

    <p>Best regards,<br />
    [Your Name]<br />
    [Your Position]<br />
    [Your Contact Information]</p>
</body>
</html>
";
                        var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                        if (string.IsNullOrEmpty(userIdString))
                        {
                            return BadRequest("User ID not found.");
                        }

                        if (!int.TryParse(userIdString, out int userId))
                        {
                            return BadRequest("Invalid User ID format.");
                        }

                        var users = await _groupMemberService.GetUsersByUserId(userId);

                        foreach (var user in users)
                        {
                            if (user.Students.FirstOrDefault()?.GroupMembers.FirstOrDefault()?.Role == "Leader")
                            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                _emailService.SendEmailAsync(user.Email, "Task of Project", taskEmail);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                                var newNotification = await _notificationService.CreateNotification(userId, user.UserId, $"Task <b>{checkComplete.TaskName}</b> has been completed.");
                                await _hubContext.Clients.User(user.UserId.ToString()).SendAsync("ReceiveNotification", newNotification);

                            }

                        }
                    }
                }

                response.Data = existingTaskDetail;
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
