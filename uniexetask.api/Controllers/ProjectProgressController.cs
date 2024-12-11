using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/project_progress")]
    [ApiController]
    public class ProjectProgressController : ControllerBase
    {
        private readonly IProjectProgressService _projectProgressService;
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
        private readonly IGroupMemberService _groupMemberService;

        public ProjectProgressController(IProjectProgressService projectProgressService,
                            ITaskService taskService,
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
                            IGroupMemberService groupMemberService)
        {
            _projectProgressService = projectProgressService;
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
            _groupMemberService = groupMemberService;
        }

        [HttpGet("byProject/{projectId}")]
        public async Task<IActionResult> GetProjectProgressByProjectId(int projectId)
        {
            ApiResponse<List<ProjectProgressEachMember>> response = new ApiResponse<List<ProjectProgressEachMember>>();

            try
            {
                List<ProjectProgressEachMember> projectData = new List<ProjectProgressEachMember>();

                var project = await _projectService.GetProjectWithAllDataById(projectId);
                if (project == null)
                {
                    throw new Exception("Project not found");
                }
                var memberGroup = await _groupMemberService.GetUsersByGroupId(project.Group.GroupId);
                
                foreach (var user in memberGroup)
                {
                    int numberOfTaskNotStarted = 0;
                    int numberOfTaskInProgress = 0;
                    int numberOfTaskCompleted = 0;
                    int numberOfTaskOverdue = 0;

                    var taskOfUser = await _taskService.GetTasksByUserId(user.UserId);
                    foreach (var task in taskOfUser)
                    {
                        if (task.Status == nameof(TasksStatus.Not_Started))
                        {
                            numberOfTaskNotStarted++;
                        }
                        else if(task.Status == nameof(TasksStatus.In_Progress))
                        {
                            numberOfTaskInProgress++;
                        }
                        else if (task.Status == nameof(TasksStatus.Completed))
                        {
                            numberOfTaskCompleted++;
                        }
                        else if (task.Status == nameof(TasksStatus.Overdue))
                        {
                            numberOfTaskOverdue++;
                        }
                    }
                    projectData.Add(new ProjectProgressEachMember
                    {
                        FullName = user.FullName,
                        Email = user.Email,
                        Avatar = user.Avatar,
                        NumberOfTaskNotStarted = numberOfTaskNotStarted,
                        NumberOfTaskInProgress = numberOfTaskInProgress,
                        NumberOfTaskCompleted = numberOfTaskCompleted,
                        NumberOfTaskOverdue = numberOfTaskOverdue,
                    });
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
    }
}
