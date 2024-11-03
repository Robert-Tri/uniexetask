using AutoMapper;
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
        private readonly IMapper _mapper;


        public TaskController(ITaskService taskService, 
                            ITaskAssignService taskAssignService, 
                            IStudentService studentsService,
                            ITaskProgressService taskProgressService,
                            IStudentService studentService,
                            IMapper mapper)
        {
            _taskService = taskService;
            _taskAssignService = taskAssignService;
            _studentsService = studentsService;
            _taskProgressService = taskProgressService;
            _studentService = studentService;
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


        [HttpGet("byUser/{userId}")]
        public async Task<IActionResult> GetTasksByUserId(int userId)
        {
            var tasksList = await _taskService.GetTasksByUserId(userId);
            if (tasksList == null)
            {
                return NotFound();
            }

            List<TaskModel> tasks = new List<TaskModel>();
            foreach (var task in tasksList)
            {
                var taskAssigns = await _taskAssignService.GetTaskAssignsByTaskId(task.TaskId);
                var taskAssignModels = taskAssigns.Select(assign => new TaskAssignModel
                {
                    TaskAssignId = assign.TaskAssignId,
                    TaskId = assign.TaskId,
                    StudentId = assign.StudentId,
                    AssignedDate = assign.AssignedDate,
                }).ToList();

                tasks.Add(new TaskModel
                {
                    TaskId = task.TaskId,
                    ProjectId = task.ProjectId,
                    TaskName = task.TaskName,
                    Description = task.Description,
                    StartDate = task.StartDate,
                    EndDate = task.EndDate,
                    Status = task.Status,
                    IsDeleted = task.IsDeleted,
                    TaskAssigns = taskAssignModels 
                });
            }

            ApiResponse<IEnumerable<TaskModel>> response = new ApiResponse<IEnumerable<TaskModel>>();
            response.Data = tasks;
            return Ok(response);
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

    }
}
