using AutoMapper;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.RegularExpressions;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;
using uniexetask.core.Models.Enums;
using Azure;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
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
        private readonly IMentorService _mentorService;
        private readonly IReqTopicService _reqTopicService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IMeetingScheduleService _meetingScheduleService;

        public DashboardController(ITaskService taskService,
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
                            IProjectProgressService projectProgressService,
                            IMentorService mentorService,
                            IReqTopicService reqTopicService,
                            IGroupMemberService groupMemberService,
                            IMeetingScheduleService meetingScheduleService)
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
            _mentorService = mentorService;
            _reqTopicService = reqTopicService;
            _groupMemberService = groupMemberService;
            _meetingScheduleService = meetingScheduleService;
        }

        [HttpGet("mentor")]
        public async Task<IActionResult> GetDashboardById()
        {
            ApiResponse<MentorDashboardModel> response = new ApiResponse<MentorDashboardModel>();

            try
            {
                var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId) || string.IsNullOrEmpty(userRole) || !userRole.Equals("Mentor"))
                {
                    return NotFound();
                }

                var mentorGroups = await _mentorService.GetMentorWithGroupAsync(userId);
                if (mentorGroups == null || mentorGroups.Groups == null)
                {
                    return NotFound("No groups found for the mentor.");
                }
                // Chuẩn bị dữ liệu Dashboard
                var groupDashboardModels = new List<GroupDashboardModel>();
                var projectDashboardModels = new List<ProjectDashboardModel>();

                foreach (var group in mentorGroups.Groups)
                {
                    if (group == null) continue;


                    // Lấy danh sách thành viên của nhóm
                    var groupMembers = await _groupMemberService.GetUsersByGroupId(group.GroupId);

                    var groupMemberModels = groupMembers.Select(member => new GroupMemberModel
                    {
                        GroupId = group.GroupId,
                        StudentId = member.Students.FirstOrDefault()?.StudentId ?? 0,
                        Role = member.Students.FirstOrDefault()?.GroupMembers.FirstOrDefault()?.Role
                    }).ToList();

                    var meetingSchedules = await _meetingScheduleService.GetMeetingSchedulesByGroupId(group.GroupId);
                    var meetingScheduleModels = meetingSchedules.Select(meeting => new MeetingScheduleDashboardModel
                    {
                        ScheduleId = meeting.ScheduleId,
                        GroupName = group.GroupName,
                        MeetingScheduleName = meeting.MeetingScheduleName,
                        Location = meeting.Location,
                        MeetingDate = meeting.MeetingDate,
                        Duration = meeting.Duration,
                        Type = meeting.Type,
                        Content = meeting.Content,
                        Url = meeting.Url ?? null,
                        IsDeleted = meeting.IsDeleted
                    }).ToList();

                    groupDashboardModels.Add(new GroupDashboardModel
                    {
                        GroupId = group.GroupId,
                        GroupName = group.GroupName,
                        SubjectId = group.SubjectId,
                        HasMentor = group.HasMentor,
                        IsCurrentPeriod = group.IsCurrentPeriod,
                        Status = group.Status,
                        IsDeleted = group.IsDeleted,
                        GroupMembers = groupMemberModels,
                        MeetingSchedules = meetingScheduleModels
                    });

                    if(group.Status == nameof(GroupStatus.Approved))
                    {
                        var projects = await _projectService.GetAllProjectsByGroupId(group.GroupId) ?? new List<Project>();

                        foreach (var project in projects)
                        {
                            if (project == null) continue;

                            var tasks = await _taskService.GetTasksByProjectId(project.ProjectId);
                            var taskDashboardModels = new List<TaskDashboardModel>();

                            foreach (var task in tasks)
                            {
                                if (task == null) continue;

                                var taskProgress = await _taskProgressService.GetTaskProgressByTaskId(task.TaskId);
                                taskDashboardModels.Add(new TaskDashboardModel
                                {
                                    TaskId = task.TaskId,
                                    Status = task.Status,
                                    ProgressPercentage = taskProgress?.ProgressPercentage ?? 0
                                });
                            }

                            Topic topic = await _topicService.GetTopicById(project.TopicId);
                            if (topic == null) continue;

                            ProjectProgress projectProgress = await _projectProgressService.GetProjectProgressByProjectId(project.ProjectId);
                            projectDashboardModels.Add(new ProjectDashboardModel
                            {
                                ProjectId = project.ProjectId,
                                TopicId = project.TopicId,
                                TopicName = topic.TopicName,
                                StartDate = project.StartDate,
                                EndDate = project.EndDate,
                                ProgressPercentage = projectProgress.ProgressPercentage,
                                SubjectId = project.SubjectId,
                                IsCurrentPeriod = project.IsCurrentPeriod,
                                Status = project.Status,
                                IsDeleted = project.IsDeleted,
                                TaskDashboards = taskDashboardModels
                            });
                        }
                    }
                }


                var regTopics = await _reqTopicService.GetAllReqTopicByMentorId(mentorGroups.MentorId);
                var regTopicDashboardModels = regTopics.Select(topic => new RegTopicDashboardModel
                {
                    RegTopicId = topic.RegTopicId,
                    TopicCode = topic.TopicCode,
                    TopicName = topic.TopicName,
                    Status = topic.Status
                }).ToList();

                // Tạo MentorDashboardModel
                var mentorDashboardModel = new MentorDashboardModel
                {
                    Groups = groupDashboardModels,
                    Projects = projectDashboardModels,
                    RegTopics = regTopicDashboardModels
                };

                response.Data = mentorDashboardModel;
                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }

        [HttpGet("manager")]
        public async Task<IActionResult> GetDashboardManager()
        {
            ApiResponse<DashboardManagerModel> response = new ApiResponse<DashboardManagerModel>();

            var users = await _userService.GetAllUsers();

            var dashboardManagerModel = new DashboardManagerModel
            {
                userHN = users.Count(u => u.CampusId == 1),
                userHCM = users.Count(u => u.CampusId == 2),
                userHD = users.Count(u => u.CampusId == 3),

                studentHN = users.Count(u => u.CampusId == 1 && u.RoleId == 3),
                studentHCM = users.Count(u => u.CampusId == 2 && u.RoleId == 3),
                studentHD = users.Count(u => u.CampusId == 3 && u.RoleId == 3),

                mentorHN = users.Count(u => u.CampusId == 1 && u.RoleId == 4),
                mentorHCM = users.Count(u => u.CampusId == 2 && u.RoleId == 4),
                mentorHD = users.Count(u => u.CampusId == 3 && u.RoleId == 4)
            };

            response.Data = dashboardManagerModel;
            response.Success = true;
            return Ok(response);
        }

    }
}
