using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using uniexetask.api.Extensions;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/task")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ITaskAssignService _taskAssignService;
        private readonly IStudentService _studentsService;
        private readonly ITaskProgressService _taskProgressService;
        private readonly IStudentService _studentService;
        private readonly ITaskDetailService _taskDetailService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly ITopicService _topicService;
        private readonly IProjectProgressService _projectProgressService;

        public TaskController(ITaskService taskService, 
                            ITaskAssignService taskAssignService, 
                            IStudentService studentsService,
                            ITaskProgressService taskProgressService,
                            IStudentService studentService,
                            ITaskDetailService taskDetailService,
                            IMapper mapper,
                            IEmailService emailService,
                            IProjectService projectService,
                            IUserService userService,
                            ITopicService topicService,
                            IProjectProgressService projectProgressService)
        {
            _taskService = taskService;
            _taskAssignService = taskAssignService;
            _studentsService = studentsService;
            _taskProgressService = taskProgressService;
            _studentService = studentService;
            _taskDetailService = taskDetailService;
            _mapper = mapper;
            _emailService = emailService;
            _projectService = projectService;
            _userService = userService;
            _topicService = topicService;
            _projectProgressService = projectProgressService;
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTasksById(int taskId)
        {
            var task = await _taskService.GetTaskById(taskId);
            if (task == null)
            {
                return NotFound();
            }
            var taskAssign = await _taskAssignService.GetTaskAssignsByTaskId(taskId);
            var taskDetail = await _taskDetailService.GetTaskDetailListByTaskId(taskId);
            var taskProgress = await _taskProgressService.GetTaskProgressByTaskId(taskId);

            task.TaskAssigns = taskAssign.ToList();
            task.TaskDetails = taskDetail.ToList();

            ApiResponse<core.Models.Task> response = new ApiResponse<core.Models.Task>();
            response.Data = task;
            return Ok(response);
        }

        [HttpGet("byProject/{projectId}")]
        public async Task<IActionResult> GetTasksByProjectId(int projectId)
        {
            ApiResponse<IEnumerable<TaskModel>> response = new ApiResponse<IEnumerable<TaskModel>>();
            try
            {
                var tasksList = await _taskService.GetTasksByProjectId(projectId);
                if (tasksList == null)
                {
                    throw new Exception("TasksList not found");
                }

                List<TaskModel> tasks = new List<TaskModel>();
                foreach (var task in tasksList)
                {
                    if (task.IsDeleted == false)
                    {
                        var taskAssigns = await _taskAssignService.GetTaskAssignsByTaskId(task.TaskId);
                        var taskAssignModels = taskAssigns.Select(assign => new TaskAssignModel
                        {
                            TaskAssignId = assign.TaskAssignId,
                            TaskId = assign.TaskId,
                            StudentId = assign.StudentId,
                            AssignedDate = assign.AssignedDate,
                        }).ToList();

                        var taskDetails = await _taskDetailService.GetTaskDetailListByTaskId(task.TaskId);
                        var taskDetailsModels = taskDetails.Select(detail => new TaskDetailsModel
                        {
                            TaskDetailId = detail.TaskDetailId,
                            TaskId = detail.TaskId,
                            TaskDetailName = detail.TaskDetailName,
                            ProgressPercentage = detail.ProgressPercentage,
                            IsCompleted = detail.IsCompleted,
                            IsDeleted = detail.IsDeleted,
                        }).ToList();

                        var taskProgress = await _taskProgressService.GetTaskProgressByTaskId(task.TaskId);

                        tasks.Add(new TaskModel
                        {
                            TaskId = task.TaskId,
                            ProjectId = task.ProjectId,
                            TaskName = task.TaskName,
                            Description = task.Description,
                            StartDate = task.StartDate,
                            EndDate = task.EndDate,
                            ProgressPercentage = taskProgress.ProgressPercentage,
                            Status = task.Status,
                            IsDeleted = task.IsDeleted,
                            TaskAssigns = taskAssignModels,
                            TaskDetails = taskDetailsModels,
                        });
                    }
                }

                response.Data = tasks;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("byProject/myTasks/{projectId}")]
        public async Task<IActionResult> GetMyTasksByProjectId(int projectId)
        {
            ApiResponse<IEnumerable<TaskModel>> response = new ApiResponse<IEnumerable<TaskModel>>();
            try
            {
                var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var studentId = await _studentService.GetStudentIdByUserId(Int32.Parse(userIdString));
                var myTaskAssignList = await _taskAssignService.GetTaskAssignsByStudent(studentId.Value);

                var tasksList = await _taskService.GetTasksByProjectId(projectId);
                if (tasksList == null)
                {
                    throw new Exception("TasksList not found");
                }

                List<TaskModel> tasks = new List<TaskModel>();
                foreach (var task in tasksList)
                {
                    if (task.IsDeleted == false)
                    {
                        foreach (var taskAss in myTaskAssignList)
                        {
                            if (taskAss.TaskId == task.TaskId)
                            {
                                var taskAssigns = await _taskAssignService.GetTaskAssignsByTaskId(task.TaskId);
                                var taskAssignModels = taskAssigns.Select(assign => new TaskAssignModel
                                {
                                    TaskAssignId = assign.TaskAssignId,
                                    TaskId = assign.TaskId,
                                    StudentId = assign.StudentId,
                                    AssignedDate = assign.AssignedDate,
                                }).ToList();

                                var taskDetails = await _taskDetailService.GetTaskDetailListByTaskId(task.TaskId);
                                var taskDetailsModels = taskDetails.Select(detail => new TaskDetailsModel
                                {
                                    TaskDetailId = detail.TaskDetailId,
                                    TaskId = detail.TaskId,
                                    TaskDetailName = detail.TaskDetailName,
                                    ProgressPercentage = detail.ProgressPercentage,
                                    IsCompleted = detail.IsCompleted,
                                    IsDeleted = detail.IsDeleted,
                                }).ToList();

                                var taskProgress = await _taskProgressService.GetTaskProgressByTaskId(task.TaskId);

                                tasks.Add(new TaskModel
                                {
                                    TaskId = task.TaskId,
                                    ProjectId = task.ProjectId,
                                    TaskName = task.TaskName,
                                    Description = task.Description,
                                    StartDate = task.StartDate,
                                    EndDate = task.EndDate,
                                    ProgressPercentage = taskProgress.ProgressPercentage,
                                    Status = task.Status,
                                    IsDeleted = task.IsDeleted,
                                    TaskAssigns = taskAssignModels,
                                    TaskDetails = taskDetailsModels,
                                });
                            }
                        }
                    }
                }

                response.Data = tasks;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("byProject/overdueIsDeleted/{projectId}")]
        public async Task<IActionResult> GetTasksOverdueIsDeletedByProjectId(int projectId)
        {
            ApiResponse<IEnumerable<TaskModel>> response = new ApiResponse<IEnumerable<TaskModel>>();
            try
            {
                var tasksList = await _taskService.GetTasksByProjectId(projectId);
                if (tasksList == null)
                {
                    throw new Exception("TasksList not found");
                }

                List<TaskModel> tasks = new List<TaskModel>();
                foreach (var task in tasksList)
                {
                    if (task.IsDeleted == true && task.Status == nameof(TasksStatus.Overdue))
                    {
                        var taskAssigns = await _taskAssignService.GetTaskAssignsByTaskId(task.TaskId);
                        var taskAssignModels = taskAssigns.Select(assign => new TaskAssignModel
                        {
                            TaskAssignId = assign.TaskAssignId,
                            TaskId = assign.TaskId,
                            StudentId = assign.StudentId,
                            AssignedDate = assign.AssignedDate,
                        }).ToList();

                        var taskDetails = await _taskDetailService.GetTaskDetailListByTaskId(task.TaskId);
                        var taskDetailsModels = taskDetails.Select(detail => new TaskDetailsModel
                        {
                            TaskDetailId = detail.TaskDetailId,
                            TaskId = detail.TaskId,
                            TaskDetailName = detail.TaskDetailName,
                            ProgressPercentage = detail.ProgressPercentage,
                            IsCompleted = detail.IsCompleted,
                            IsDeleted = detail.IsDeleted,
                        }).ToList();

                        var taskProgress = await _taskProgressService.GetTaskProgressByTaskId(task.TaskId);

                        tasks.Add(new TaskModel
                        {
                            TaskId = task.TaskId,
                            ProjectId = task.ProjectId,
                            TaskName = task.TaskName,
                            Description = task.Description,
                            StartDate = task.StartDate,
                            EndDate = task.EndDate,
                            ProgressPercentage = taskProgress.ProgressPercentage,
                            Status = task.Status,
                            IsDeleted = task.IsDeleted,
                            TaskAssigns = taskAssignModels,
                            TaskDetails = taskDetailsModels,
                        });
                    }
                }

                response.Data = tasks;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("byUser/{userId}")]
        public async Task<IActionResult> GetTasksByUserId(int userId)
        {
            ApiResponse<IEnumerable<TaskModel>> response = new ApiResponse<IEnumerable<TaskModel>>();
            try
            {
                var tasksList = await _taskService.GetTasksByUserId(userId);
                if (tasksList == null)
                {
                    throw new Exception("TasksList not found");
                }

                List<TaskModel> tasks = new List<TaskModel>();
                foreach (var task in tasksList)
                {
                    if (!task.IsDeleted)
                    {
                        var taskAssigns = await _taskAssignService.GetTaskAssignsByTaskId(task.TaskId);

                        var taskAssignModels = taskAssigns.Select(assign => new TaskAssignModel
                        {
                            TaskAssignId = assign.TaskAssignId,
                            TaskId = assign.TaskId,
                            StudentId = assign.StudentId,
                            AssignedDate = assign.AssignedDate,
                        }).ToList();

                        var taskDetails = await _taskDetailService.GetTaskDetailListByTaskId(task.TaskId);
                        var taskDetailsModels = taskDetails.Select(detail => new TaskDetailsModel
                        {
                            TaskDetailId = detail.TaskDetailId,
                            TaskId = detail.TaskId,
                            TaskDetailName = detail.TaskDetailName,
                            ProgressPercentage = detail.ProgressPercentage,
                            IsCompleted = detail.IsCompleted,
                            IsDeleted = detail.IsDeleted,
                        }).ToList();

                        var taskProgress = await _taskProgressService.GetTaskProgressByTaskId(task.TaskId);

                        tasks.Add(new TaskModel
                        {
                            TaskId = task.TaskId,
                            ProjectId = task.ProjectId,
                            TaskName = task.TaskName,
                            Description = task.Description,
                            StartDate = task.StartDate,
                            EndDate = task.EndDate,
                            ProgressPercentage = taskProgress.ProgressPercentage,
                            Status = task.Status,
                            IsDeleted = task.IsDeleted,
                            TaskAssigns = taskAssignModels,
                            TaskDetails = taskDetailsModels,
                        });
                    }
                }

                response.Data = tasks;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskModel task)
        {
            ApiResponse<core.Models.Task> response = new ApiResponse<core.Models.Task>();
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("ModelState not found");
                }

                if (task.TaskDetailsModel == null || !task.TaskDetailsModel.Any())
                {
                    throw new Exception("Error: TaskDetailsModel is empty or null.");
                }

                decimal totalProgressPercentage = 0;
                foreach (var taskDetailModel in task.TaskDetailsModel)
                {
                    totalProgressPercentage += taskDetailModel.ProgressPercentage;
                }

                if (Math.Round(totalProgressPercentage, 2) != 100)
                {
                    throw new Exception("Error: Total ProgressPercentage is NOT equal to 100");
                }

                var obj = _mapper.Map<core.Models.Task>(task);

                var createdTask = await _taskService.CreateTask(obj);
                if (!createdTask)
                {
                    throw new Exception("Error creating task");
                }
                var taskId = obj.TaskId;
                foreach (var studentId in task.AssignedMembers)
                {
                    var createdTaskAssign = await _taskAssignService.CreateTaskAssign(new TaskAssign
                    {
                        TaskId = taskId,
                        StudentId = studentId,
                        AssignedDate = DateTime.Now
                    });
                    if (createdTaskAssign == null)
                    {
                        throw new Exception("Error creating task");
                    }
                }
              
                foreach (var taskDetailModel in task.TaskDetailsModel)
                {
                    var createdTaskDetails = await _taskDetailService.CreateTaskDetails(new TaskDetail
                    {
                        TaskId = taskId,
                        TaskDetailName = taskDetailModel.TaskDetailName,
                        ProgressPercentage = taskDetailModel.ProgressPercentage,
                        IsCompleted = false,
                        IsDeleted = false,
                    });
                    if (createdTaskDetails == null)
                    {
                        throw new Exception("Error creating task");
                    }
                }
                // Kiểm tra cập nhật Progress và Status
                var loadProgressTaskProgress = await _taskProgressService.LoadProgressUpdateTaskProgressByTaskId(taskId);
                var projectId = task.ProjectId;
                var loadProgressProject = await _projectProgressService.LoadProgressUpdateProjectProgressByProjectId(projectId);

                //Gửi mail
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
    </style>
</head>
<body>
    <h2>Dear EXE Students,</h2>

    <p>You have been assigned a new task in your project.</p>

    <p><strong>Task Information:</strong></p>
    <p><strong>Name:</strong> {obj.TaskName}</p>
    <p><strong>Start Date:</strong> {obj.StartDate.ToString("MMMM dd, yyyy")}</p>
    <p><strong>Deadline:</strong> {obj.EndDate.ToString("MMMM dd, yyyy")}</p>
    <p><strong>Description:</strong> {obj.Description}</p>

    <p>This is an automated email. Please do not reply to this email.</p>
    <p>Looking forward to your participation.</p>

    <p>Best regards,<br />
    [Your Name]<br />
    [Your Position]<br />
    [Your Contact Information]</p>
</body>
</html>
";
                var taskAssigns = await _taskAssignService.GetTaskAssignsByTaskId(taskId);
                List<string> emailList = new List<string>();
                foreach (var taskAssign in taskAssigns)
                {
                    var student = await _studentService.GetStudentById(taskAssign.StudentId);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _emailService.SendEmailAsync(student.User.Email, "Task of Project", taskEmail);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                }

                response.Data = obj;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskModel task)
        {
            ApiResponse<core.Models.Task> response = new ApiResponse<core.Models.Task>();
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("ModelState not found");
                }

                decimal totalProgressPercentage = 0;
                foreach (var taskDetailModel in task.TaskDetailsModel)
                {
                    totalProgressPercentage += taskDetailModel.ProgressPercentage;
                }

                if (Math.Round(totalProgressPercentage, 2) != 100)
                {
                    throw new Exception("Error: Total ProgressPercentage is NOT equal to 100");
                }
                if (task.EndDate <= DateTime.Now) throw new Exception("End date exceeds current date.");

                // Tìm Task hiện tại theo TaskId
                var existingTask = await _taskService.GetTaskById(task.TaskId);
                if (existingTask == null)
                {
                    throw new Exception("Task not found");
                }

                existingTask.TaskName = task.TaskName;
                existingTask.Description = task.Description;
                existingTask.StartDate = task.StartDate;
                existingTask.EndDate = task.EndDate;

                var taskId = task.TaskId;  // Lấy TaskId từ task đã cập nhật

                var updatedTask = await _taskService.UpdateTask(existingTask);
                if (!updatedTask)
                {
                    throw new Exception("Error updating task");
                }

                // Xóa các TaskAssign hiện tại để thêm mới các thành viên được giao nhiệm vụ
                var deletedTaskAssigns = await _taskAssignService.DeleteTaskAssignByTaskId(taskId);
                if (!deletedTaskAssigns)
                {
                    throw new Exception("Error updating task assigns");
                }

                // Tạo lại TaskAssign cho các thành viên được chỉ định
                foreach (var studentId in task.AssignedMembers)
                {
                    var createdTaskAssign = await _taskAssignService.CreateTaskAssign(new TaskAssign
                    {
                        TaskId = taskId,
                        StudentId = studentId,
                        AssignedDate = DateTime.Now
                    });

                    if (createdTaskAssign == null)
                    {
                        throw new Exception("Error updating task assigns");
                    }
                }

                var deletedTaskDetails = await _taskDetailService.DeleteTaskDetailsByTaskId(taskId);
                if (!deletedTaskAssigns)
                {
                    throw new Exception("Error updating task assigns");
                }
                foreach (var taskDetailModel in task.TaskDetailsModel)
                {
                    var createdTaskDetails = await _taskDetailService.CreateTaskDetails(new TaskDetail
                    {
                        TaskId = taskId,
                        TaskDetailName = taskDetailModel.TaskDetailName,
                        ProgressPercentage = taskDetailModel.ProgressPercentage,
                        IsCompleted = false,
                        IsDeleted = false,
                    });
                    if (createdTaskDetails == null)
                    {
                        throw new Exception("Error creating task");
                    }
                }
                // Kiểm tra cập nhật Progress và Status
                var loadProgressTaskProgress = await _taskProgressService.LoadProgressUpdateTaskProgressByTaskId(taskId);
                var projectId = task.ProjectId;
                var loadProgressProject = await _projectProgressService.LoadProgressUpdateProjectProgressByProjectId(projectId);

                //Gửi mail
                var taskDeltailList = await _taskDetailService.GetTaskDetailListByTaskId(taskId);

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

    <p>One of your assigned tasks in the project has been updated by the Leader!</p>

    <p><strong>Task Information:</strong></p>
    <p><strong>Name:</strong> {existingTask.TaskName}</p>
    <p><strong>Start Date:</strong> {existingTask.StartDate.ToString("MMMM dd, yyyy")}</p>
    <p><strong>Deadline:</strong> {existingTask.EndDate.ToString("MMMM dd, yyyy")}</p>
    <p><strong>Description:</strong> {existingTask.Description}</p>
    
    <p><strong>Details:</strong></p>
    <table>
        <thead>
            <tr>
                <th>Task Detail Name</th>
                <th>Progress Percentage</th>
            </tr>
        </thead>
        <tbody>
";
                foreach (var item in taskDeltailList)
                {
                    taskEmail += $@"
            <tr>
                <td>{item.TaskDetailName}</td>
                <td>{item.ProgressPercentage}%</td>
            </tr>";
                }

                taskEmail += @"
        </tbody>
    </table>

    <p>This is an automated email. Please do not reply to this email.</p>
    <p>Looking forward to your participation.</p>

    <p>Best regards,<br />
    [Your Name]<br />
    [Your Position]<br />
    [Your Contact Information]</p>
</body>
</html>
";

                var taskAssigns = await _taskAssignService.GetTaskAssignsByTaskId(taskId);

                List<string> emailList = new List<string>();
                foreach (var taskAssign in taskAssigns)
                {
                    var student = await _studentService.GetStudentById(taskAssign.StudentId);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _emailService.SendEmailAsync(student.User.Email, "Task of Project", taskEmail);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                }

                response.Data = existingTask;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                // Kiểm tra giá trị taskId hợp lệ
                if (taskId <= 0)
                {
                    response.Success = false;
                    response.ErrorMessage = "Invalid task ID.";
                    return BadRequest(response);
                }
                var task = await _taskService.GetTaskById(taskId);
                // Gọi phương thức DeleteTask từ TaskService
                var result = await _taskService.DeleteTask(taskId);
                if (result)
                {
                    // Kiểm tra cập nhật Progress và Status
                    var loadProgressTaskProgress = await _taskProgressService.LoadProgressUpdateTaskProgressByTaskId(taskId);
                    var projectId = task.ProjectId;
                    var loadProgressProject = await _projectProgressService.LoadProgressUpdateProjectProgressByProjectId(projectId);

                    //Gửi mail
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

    <p>A task assigned to you in the project has been DELETED by the Leader!</p>

    <p><strong>Task Information:</strong></p>
    <p><strong>Name:</strong> {task.TaskName}</p>
    <p><strong>Start Date:</strong> {task.StartDate.ToString("MMMM dd, yyyy")}</p>
    <p><strong>Deadline:</strong> {task.EndDate.ToString("MMMM dd, yyyy")}</p>
    <p><strong>Description:</strong> {task.Description}</p>
    

    <p>This is an automated email. Please do not reply to this email.</p>
    <p>Looking forward to your participation.</p>

    <p>Best regards,<br />
    [Your Name]<br />
    [Your Position]<br />
    [Your Contact Information]</p>
</body>
</html>
";

                    var taskAssigns = await _taskAssignService.GetTaskAssignsByTaskId(taskId);
                    List<string> emailList = new List<string>();
                    foreach (var taskAssign in taskAssigns)
                    {
                        var student = await _studentService.GetStudentById(taskAssign.StudentId);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        _emailService.SendEmailAsync(student.User.Email, "Task of Project", taskEmail);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                    }

                    response.Success = true;
                    response.Data = "Task deleted successfully.";
                    return Ok(response);
                }
                else
                {
                    response.Success = false;
                    response.ErrorMessage = "Task not found.";
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
        public async Task<IActionResult> UpdateStatusTask([FromBody] UpdateStatusTaskModel model)
        {
            ApiResponse<core.Models.Task> response = new ApiResponse<core.Models.Task>();
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (!ModelState.IsValid)
                {
                    throw new Exception("Invalid request model");
                }

                // Lấy task từ database theo taskId
                var existingTask = await _taskService.GetTaskById(model.TaskId);
                if (existingTask == null)
                {
                    response.Success = false;
                    response.ErrorMessage = "Task not found.";
                    return NotFound(response);
                }

                // Cập nhật trạng thái của task
                existingTask.Status = model.Status;

                // Lưu thay đổi vào cơ sở dữ liệu
                var updatedTask = await _taskService.UpdateTask(existingTask);
                if (!updatedTask)
                {
                    throw new Exception("Error updating task status");
                }

                // Kiểm tra cập nhật Progress và Status
                var loadProgressTaskProgress = await _taskProgressService.LoadProgressUpdateTaskProgressByTaskId(existingTask.TaskId);
                var projectId = existingTask.ProjectId;
                var loadProgressProject = await _projectProgressService.LoadProgressUpdateProjectProgressByProjectId(projectId);

                //Gửi mail
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

    <p>A task assigned to you in the project has been DELETED by the Leader!</p>

    <p><strong>Task Information:</strong></p>
    <p><strong>Name:</strong> {existingTask.TaskName}</p>
    <p><strong>Start Date:</strong> {existingTask.StartDate.ToString("MMMM dd, yyyy")}</p>
    <p><strong>Deadline:</strong> {existingTask.EndDate.ToString("MMMM dd, yyyy")}</p>
    <p><strong>Description:</strong> {existingTask.Description}</p>
    

    <p>This is an automated email. Please do not reply to this email.</p>
    <p>Looking forward to your participation.</p>

    <p>Best regards,<br />
    [Your Name]<br />
    [Your Position]<br />
    [Your Contact Information]</p>
</body>
</html>
";

                var taskAssigns = await _taskAssignService.GetTaskAssignsByTaskId(existingTask.TaskId);
                List<string> emailList = new List<string>();
                foreach (var taskAssign in taskAssigns)
                {
                    var student = await _studentService.GetStudentById(taskAssign.StudentId);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _emailService.SendEmailAsync(student.User.Email, "Task of Project", taskEmail);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                }

                // Trả về task sau khi cập nhật
                response.Data = existingTask;
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

        [HttpPut("re-assign")]
        public async Task<IActionResult> ReAssignTask([FromBody] UpdateTaskModel task)
        {
            ApiResponse<core.Models.Task> response = new ApiResponse<core.Models.Task>();
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("ModelState not found");
                }

                decimal totalProgressPercentage = 0;
                foreach (var taskDetailModel in task.TaskDetailsModel)
                {
                    totalProgressPercentage += taskDetailModel.ProgressPercentage;
                }

                if (Math.Round(totalProgressPercentage, 2) != 100)
                {
                    throw new Exception("Error: Total ProgressPercentage is NOT equal to 100");
                }
                if (task.EndDate <= DateTime.Now) throw new Exception("End date exceeds current date.");

                // Tìm Task hiện tại theo TaskId
                var existingTask = await _taskService.GetTaskById(task.TaskId);
                if (existingTask == null)
                {
                    throw new Exception("Task not found");
                }

                //Nếu Overdue thì Re-Assign
                if (existingTask.Status == nameof(TasksStatus.Overdue))
                {
                    var deletesTask = await _taskService.DeleteTask(existingTask.TaskId);
                    if (!deletesTask)
                    {
                        throw new Exception("Error re-assign task");
                    }

                    core.Models.Task taskModel = new core.Models.Task();

                    taskModel.ProjectId = task.ProjectId;
                    taskModel.TaskName = task.TaskName;
                    taskModel.Description = task.Description;
                    taskModel.StartDate = task.StartDate;
                    taskModel.EndDate = task.EndDate;

                    var createTask = await _taskService.CreateTask(taskModel);
                    if (!createTask)
                    {
                        throw new Exception("Error re-assign task");
                    }

                    var taskIdNew = taskModel.TaskId;
                    // Tạo TaskAssign cho Task mới
                    foreach (var studentId in task.AssignedMembers)
                    {
                        var createdTaskAssign = await _taskAssignService.CreateTaskAssign(new TaskAssign
                        {
                            TaskId = taskIdNew,
                            StudentId = studentId,
                            AssignedDate = DateTime.Now
                        });

                        if (createdTaskAssign == null)
                        {
                            throw new Exception("Error re-assign task");
                        }
                    }

                    var deletedTaskDetails = await _taskDetailService.DeleteTaskDetailsByTaskId(existingTask.TaskId);
                    if (!deletedTaskDetails)
                    {
                        throw new Exception("Error re-assign task");
                    }
                    foreach (var taskDetailModel in task.TaskDetailsModel)
                    {
                        var createdTaskDetails = await _taskDetailService.CreateTaskDetails(new TaskDetail
                        {
                            TaskId = taskIdNew,
                            TaskDetailName = taskDetailModel.TaskDetailName,
                            ProgressPercentage = taskDetailModel.ProgressPercentage,
                            IsCompleted = false,
                            IsDeleted = false,
                        });
                        if (createdTaskDetails == null)
                        {
                            throw new Exception("Error re-assign task");
                        }
                    }
                    // Kiểm tra cập nhật Progress và Status
                    var loadProgressTaskProgress = await _taskProgressService.LoadProgressUpdateTaskProgressByTaskId(taskIdNew);
                    var projectId = task.ProjectId;
                    var loadProgressProject = await _projectProgressService.LoadProgressUpdateProjectProgressByProjectId(projectId);
               
                    //Gửi mail
                    var taskDeltailList = await _taskDetailService.GetTaskDetailListByTaskId(taskIdNew);

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

    <p>You have been assigned a new task in your project.</p>

    <p><strong>Task Information:</strong></p>
    <p><strong>Name:</strong> {taskModel.TaskName}</p>
    <p><strong>Start Date:</strong> {taskModel.StartDate.ToString("MMMM dd, yyyy")}</p>
    <p><strong>Deadline:</strong> {taskModel.EndDate.ToString("MMMM dd, yyyy")}</p>
    <p><strong>Description:</strong> {taskModel.Description}</p>
    
    <p><strong>Details:</strong></p>
    <table>
        <thead>
            <tr>
                <th>Task Detail Name</th>
                <th>Progress Percentage</th>
            </tr>
        </thead>
        <tbody>
";
                foreach (var item in taskDeltailList)
                {
                    taskEmail += $@"
            <tr>
                <td>{item.TaskDetailName}</td>
                <td>{item.ProgressPercentage}%</td>
            </tr>";
                }

                taskEmail += @"
        </tbody>
    </table>

    <p>This is an automated email. Please do not reply to this email.</p>
    <p>Looking forward to your participation.</p>

    <p>Best regards,<br />
    [Your Name]<br />
    [Your Position]<br />
    [Your Contact Information]</p>
</body>
</html>
";

                    var taskAssigns = await _taskAssignService.GetTaskAssignsByTaskId(taskIdNew);

                    List<string> emailList = new List<string>();
                    foreach (var taskAssign in taskAssigns)
                    {
                        var student = await _studentService.GetStudentById(taskAssign.StudentId);

    #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        _emailService.SendEmailAsync(student.User.Email, "Task of Project", taskEmail);
    #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                    }

                    response.Data = taskModel;
                    return Ok(response);
                }
                else
                {
                    throw new Exception("Error re-assign task");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }
    }
}
