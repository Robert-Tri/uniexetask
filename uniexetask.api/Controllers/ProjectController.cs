using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IMentorService _mentorService;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;

        public ProjectController(IProjectService projectService, IGroupService groupService, IMentorService mentorService, IUserService userService, IStudentService studentService)
        {
            _projectService = projectService;
            _mentorService = mentorService;
            _groupService = groupService;
            _userService = userService;
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectList()
        {
            var projectsList = await _projectService.GetAllProjects();
            if (projectsList == null)
            {
                return NotFound();
            }
            List<ProjectListModel> projects = new List<ProjectListModel>();
            foreach (var project in projectsList)
            {
                if (project != null && project.IsCurrentPeriod) projects.Add(new ProjectListModel
                {
                    ProjectId = project.ProjectId,
                    TopicCode = project.Topic.TopicCode,
                    TopicName = project.Topic.TopicName,
                    Description = project.Topic.Description,
                    SubjectName = project.Subject.SubjectName,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    Status = project.Status,
                    IsDeleted = project.IsDeleted
                });
            }
            ApiResponse<IEnumerable<ProjectListModel>> response = new ApiResponse<IEnumerable<ProjectListModel>>();
            response.Data = projects;
            return Ok(response);
        }

        [Authorize(Roles = "Mentor")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetProjectsPendingWithMentor()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId) || string.IsNullOrEmpty(userRole) || !userRole.Equals("Mentor"))
            {
                return NotFound();
            }
            var mentor = await _mentorService.GetMentorWithGroupAsync(userId);
            if (mentor == null) return NotFound();
            Project? project = new Project();
            List<ProjectPendingModel> projects = new List<ProjectPendingModel>();
            foreach (var item in mentor.Groups)
            {
                project = await _projectService.GetProjectPendingByGroupAsync(item);
                if (project != null) projects.Add(new ProjectPendingModel
                {
                    id = project.ProjectId.ToString(),
                    GroupName = item.GroupName,
                    Topic = project.Topic.TopicName,
                    Description = project.Topic.Description,
                    Status = project.Status
                });
            }
            ApiResponse<IEnumerable<ProjectPendingModel>> response = new ApiResponse<IEnumerable<ProjectPendingModel>>();
            response.Data = projects;
            return Ok(response);
        }

        [Authorize(Roles = "4")]
        [HttpPost("projects/{projectIdStr}/update-status")]
        public async Task<IActionResult> UpdateProjectStatus(string projectIdStr, [FromBody] string action)
        {
            if (string.IsNullOrEmpty(projectIdStr) || !int.TryParse(projectIdStr, out int projectId) || string.IsNullOrEmpty(action))
            {
                return NotFound();
            }
            var result = await _projectService.UpdateProjectStatus(projectId, action);
            if (result) return Ok("Done");
            else return BadRequest();
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetProjectByUserId(int userId)
        {
            ApiResponse<ProjectListModel> response = new ApiResponse<ProjectListModel>();

            try
            {
                var student = await _studentService.GetStudentByUserId(userId);

                if (student == null)
                {
                    throw new Exception("Student not found");
                }

                var project = await _projectService.GetProjectByStudentId(student.StudentId);
                if (project == null)
                {
                    throw new Exception("Project not found");
                }
                ProjectListModel projectData = new ProjectListModel();

                projectData = (new ProjectListModel
                {
                    ProjectId = project.ProjectId,
                    TopicCode = project.Topic.TopicCode,
                    TopicName = project.Topic.TopicName,
                    Description = project.Topic.Description,
                    SubjectName = project.Subject.SubjectName,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    Status = project.Status,
                });
                foreach (var item in project.ProjectProgresses)
                {
                    if (!item.IsDeleted)
                    {
                        projectData.ProgressPercentage = item.ProgressPercentage; // Thêm item vào danh sách
                    }
                }
                // Trả về dữ liệu project
                response.Data = projectData;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("getProjectId/{userId}")]
        public async Task<IActionResult> GetProjectIdByUserId(int userId)
        {
            ApiResponse<Int32> response = new ApiResponse<Int32>();
            try
            {
                var student = await _studentService.GetStudentByUserId(userId);

                if (student == null)
                {
                    throw new Exception("Student not found");
                }

                // Gọi phương thức service để lấy project của user
                var project = await _projectService.GetProjectByStudentId(student.StudentId);

                if (project == null)
                {
                    throw new Exception("Project not found");
                }
                // Trả về dữ liệu project
                response.Data = project.ProjectId;
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
