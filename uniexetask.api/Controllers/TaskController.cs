using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using uniexetask.api.Models.Request;
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var obj = _mapper.Map<core.Models.Task>(task);

            var createdTask = await _taskService.CreateTask(obj);
            if (!createdTask)
            {
                return StatusCode(500, "Error creating task");
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
                    return StatusCode(500, "Error creating task");
                }
            }
            return Ok(obj);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskModel task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Tìm Task hiện tại theo TaskId
            var existingTask = await _taskService.GetTaskById(task.TaskId);
            if (existingTask == null)
            {
                return NotFound("Task not found");
            }

            // Cập nhật thông tin task từ UpdateTaskModel
            _mapper.Map(task, existingTask);

            var updatedTask = await _taskService.UpdateTask(existingTask);
            if (!updatedTask)
            {
                return StatusCode(500, "Error updating task");
            }

            var taskId = task.TaskId;  // Lấy TaskId từ task đã cập nhật

            // Xóa các TaskAssign hiện tại để thêm mới các thành viên được giao nhiệm vụ
            var deletedTaskAssigns = await _taskAssignService.DeleteTaskAssignByTaskId(taskId);
            if (!deletedTaskAssigns)
            {
                return StatusCode(500, "Error updating task assigns");
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
                    return StatusCode(500, "Error updating task assigns");
                }
            }

            return Ok(existingTask);
        }

    }
}
