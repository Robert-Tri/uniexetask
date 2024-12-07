using AutoMapper;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.RegularExpressions;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

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

                // Chuẩn bị dữ liệu Dashboard
                var groupDashboardModels = new List<GroupDashboardModel>();

                foreach (var group in mentorGroups.Groups)
                {
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
                }

                var projectDashboardModels = new List<ProjectDashboardModel>();

                foreach (var group in mentorGroups.Groups)
                {
                    var projects = await _projectService.GetAllProjectsByGroupId(group.GroupId);

                    foreach (var project in projects)
                    {
                        var tasks = await _taskService.GetTasksByProjectId(project.ProjectId);
                        var taskDashboardModels = new List<TaskDashboardModel>();

                        foreach (var task in tasks)
                        {
                            var taskProgress = await _taskProgressService.GetTaskProgressByTaskId(task.TaskId);
                            taskDashboardModels.Add(new TaskDashboardModel
                            {
                                TaskId = task.TaskId,
                                Status = task.Status,
                                ProgressPercentage = taskProgress?.ProgressPercentage ?? 0
                            });
                        }

                        Topic topic = await _topicService.GetTopicById(project.TopicId);
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



    }
}
