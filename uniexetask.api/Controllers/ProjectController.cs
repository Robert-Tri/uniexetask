using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;
using uniexetask.core.Models.Enums;

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
        private readonly ITimeLineService _timelineService;

        public ProjectController(IProjectService projectService, IGroupService groupService, IMentorService mentorService, IUserService userService, IStudentService studentService, ITimeLineService timelineService)
        {
            _projectService = projectService;
            _mentorService = mentorService;
            _groupService = groupService;
            _userService = userService;
            _studentService = studentService;
            _timelineService = timelineService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectList()
        {
            var projectsList = await _projectService.GetAllProjectsIsCurrentPeriod();
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
                    SubjectId = project.Subject.SubjectId,
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
        [HttpGet("GetProjectByMentor")]
        public async Task<IActionResult> GetProjectByMentor()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId) || string.IsNullOrEmpty(userRole) || !userRole.Equals("Mentor"))
            {
                return NotFound();
            }

            var mentor = await _mentorService.GetMentorWithGroupAsync(userId);

            if (mentor == null || !mentor.Groups.Any())
            {
                return NotFound("No groups found.");
            }

            var groupIds = mentor.Groups.Select(g => g.GroupId).ToList();

            var projects = new List<object>();

            foreach (var groupId in groupIds)
            {
                var projectList = await _projectService.GetAllProjectsByGroupId(groupId);
                if (projectList != null && projectList.Any())
                {
                    foreach (var project in projectList)
                    {
                        var projectData = new
                        {
                            ProjectId = project.ProjectId,
                            GroupName = project.Group.GroupName,
                            TopicId = project.Topic?.TopicId,
                            TopicCode = project.Topic?.TopicCode,
                            TopicName = project.Topic?.TopicName,
                            Description = project.Topic?.Description,
                            SubjectName = project.Subject?.SubjectName,
                            SubjectId = project.Subject?.SubjectId,
                            StartDate = project.StartDate,
                            EndDate = project.EndDate,
                            Status = project.Status,
                            IsDeleted = project.IsDeleted
                        };

                        projects.Add(projectData);
                    }
                }
            }

            if (!projects.Any())
            {
                return NotFound("No projects found.");
            }

            var response = new ApiResponse<IEnumerable<object>>()
            {
                Data = projects
            };

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

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProjectById(int projectId)
        {
            ApiResponse<ProjectListModel> response = new ApiResponse<ProjectListModel>();

            try
            {
                var project = await _projectService.GetProjectWithAllDataById(projectId);
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
        /*[Authorize(nameof(EnumRole.Student))]
        [HttpPut("continueproject")]
        public async Task<IActionResult> ContinueProject(int userId)
        {
            var timeLineEndDurationEXE101 = await _timelineService.GetTimelineById((int)TimelineType.CurrentTermDurationEXE101);
            if (DateTime.Today.Date < timeLineEndDurationEXE101.EndDate.AddDays(-14))
            {
                return BadRequest(new ApiResponse<Project> { Success = false, ErrorMessage = "It is not yet the scheduled date to update the status of the group and project." });
            }
            else if (DateTime.Today.Date > timeLineEndDurationEXE101.EndDate)
            {
                return BadRequest(new ApiResponse<Project> { Success = false, ErrorMessage = "The scheduled date to update the status of the group and project has passed." });
            }
            var result = await _projectService.ContinueProject(userId);
            ApiResponse<bool> respone = new ApiResponse<bool>();
            respone.Data = result;
            return Ok(respone);
        }*/
    }
}
