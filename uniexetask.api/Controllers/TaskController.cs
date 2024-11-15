using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
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


        public TaskController(ITaskService taskService, 
                            ITaskAssignService taskAssignService, 
                            IStudentService studentsService,
                            ITaskProgressService taskProgressService,
                            IStudentService studentService,
                            ITaskDetailService taskDetailService,
                            IMapper mapper)
        {
            _taskService = taskService;
            _taskAssignService = taskAssignService;
            _studentsService = studentsService;
            _taskProgressService = taskProgressService;
            _studentService = studentService;
            _taskDetailService = taskDetailService;
            _mapper = mapper;

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

                var updatedTask = await _taskService.UpdateTask(existingTask);
                if (!updatedTask)
                {
                    throw new Exception("Error updating task");
                }

                var taskId = task.TaskId;  // Lấy TaskId từ task đã cập nhật

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

                response.Data = existingTask;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
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

                // Gọi phương thức DeleteTask từ TaskService
                var result = await _taskService.DeleteTask(taskId);
                if (result)
                {
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

    }
}
