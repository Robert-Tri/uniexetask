using AutoMapper;
using Google.Api.Gax;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using uniexetask.api.Hubs;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/meeting-schedules")]
    [ApiController]
    public class MeetingScheduleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMeetingScheduleService _meetingScheduleService;
        private readonly ITopicService _topicService;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        private readonly IMentorService _mentorService;
        private readonly IProjectService _projectService;
        private readonly IStudentService _studentService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public MeetingScheduleController(
            IMapper mapper,
            IMeetingScheduleService meetingScheduleService, 
            ITopicService topicService, 
            IGroupService groupServic, 
            IUserService userService, 
            IMentorService mentorService,
            IProjectService projectService,
            IStudentService studentService,
            IGroupMemberService groupMemberService,
            INotificationService notificationService,
            IHubContext<NotificationHub> hubContext)
        {
            _mapper = mapper;
            _meetingScheduleService = meetingScheduleService;
            _topicService = topicService;
            _groupService = groupServic;
            _userService = userService;
            _mentorService = mentorService;
            _projectService = projectService;
            _studentService = studentService;
            _groupMemberService = groupMemberService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetEvents()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            ApiResponse<IEnumerable<MeetingScheduleResponse>> response = new ApiResponse<IEnumerable<MeetingScheduleResponse>>();
            List<MeetingScheduleResponse> list = new List<MeetingScheduleResponse>();
            try
            {
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid User Id");
                }
                if (string.IsNullOrEmpty(role))
                {
                    throw new Exception("Invalid Role");
                }
                if (role == nameof(EnumRole.Mentor))
                {
                    var mentorWithGroups = await _mentorService.GetMentorWithGroupAsync(userId);
                    if (mentorWithGroups == null) throw new Exception("There is no mentor with the provided user id."); 
                    foreach (var group in mentorWithGroups.Groups)
                    {
                        if (group.IsDeleted) continue;
                        var project = await _projectService.GetProjectWithTopicByGroupId(group.GroupId);
                        var meetingSchedules = await _meetingScheduleService.GetMeetingSchedulesByGroupId(group.GroupId);
                        if (meetingSchedules == null) continue;
                        foreach (var meeting in meetingSchedules)
                        {
                            if (meeting.MentorId != mentorWithGroups.MentorId) continue;
                            var user = await _userService.GetUserById(mentorWithGroups.UserId);
                            if (user == null) throw new Exception("user not found from mentor.");
                            list.Add(GetMeetingScheduleResponse(meeting, project, group, user));
                        }
                    }
                }
                else if (role == nameof(EnumRole.Student))
                {
                    var student = await _studentService.GetStudentByUserId(userId);
                    if (student == null) throw new Exception("There is no student with the provided user id.");
                    var groupMember = await _groupMemberService.GetGroupByStudentId(student.StudentId);
                    if (groupMember == null) throw new Exception("You do not have a group");
                    var group = await _groupService.GetGroupById(groupMember.GroupId);
                    if (group == null) throw new Exception("Group not found");
                    if (!group.IsDeleted)
                    {
                        var project = await _projectService.GetProjectWithTopicByGroupId(group.GroupId);
                        var meetingSchedules = await _meetingScheduleService.GetMeetingSchedulesByGroupId(group.GroupId);
                        if (meetingSchedules == null) throw new Exception("Load data failed.");
                        foreach (var meeting in meetingSchedules)
                        {
                            var mentor = await _mentorService.GetMentorById(meeting.MentorId);
                            if (mentor == null) 
                            { 
                                System.Diagnostics.Debug.WriteLine($"Mentor not found from mentorid = {meeting.GroupId} in schedule.");
                                continue;
                            }
                            var user = await _userService.GetUserById(mentor.UserId);
                            if (user == null) 
                            {
                                System.Diagnostics.Debug.WriteLine($"User not found from mentorid = {mentor.UserId}");
                                continue;
                            }
                            list.Add(GetMeetingScheduleResponse(meeting, project, group, user));
                        }
                    }
                }
                response.Data = list;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateMeeting([FromBody] MeetingScheduleModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            ApiResponse<MeetingScheduleResponse> response = new ApiResponse<MeetingScheduleResponse>();
            try
            {
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid User Id");
                }
                if (model.MeetingDate < DateTime.Now) throw new Exception("Cannot create event in past time.");
                if (role != nameof(EnumRole.Mentor)) throw new Exception("You are not a mentor.");
                var mentor = await _mentorService.GetMentorByUserId(userId);
                if (mentor == null) throw new Exception("There is no mentor corresponding to the provided user.");
                model.MentorId = mentor.MentorId;
                var meetingSchedule = _mapper.Map<MeetingSchedule>(model);
                if (meetingSchedule == null) throw new Exception("Invalid model.");
                var group = await _groupService.GetGroupById(meetingSchedule.GroupId);
                if (group == null) throw new Exception("You are creating an meeting for a group that does not exist.");
                if (group.Status == nameof(GroupStatus.Initialized)) throw new Exception($"{group.GroupName} Group is in newly created state and has not been assigned a mentor yet.");
                var newMeetingSchedule = await _meetingScheduleService.CreateMeetingSchedule(meetingSchedule);
                if (newMeetingSchedule == null) throw new Exception("Create meeting failed");
                var users = await _groupMemberService.GetUsersByGroupId(group.GroupId);
                foreach (var user in users)
                {
                    string formattedDate = newMeetingSchedule.MeetingDate.ToString("hh:mm tt on MM/dd");
                    var newNotification = await _notificationService.CreateNotification(userId, user.UserId,
                            $"A meeting schedule with your group has been created at {formattedDate}, please go to meeting schedule to view it.");
                    await _hubContext.Clients.User(user.UserId.ToString()).SendAsync("ReceiveNotification", newNotification);
                }
                var project = await _projectService.GetProjectWithTopicByGroupId(newMeetingSchedule.GroupId);
                group = await _groupService.GetGroupById(newMeetingSchedule.GroupId);
                var userWithRoleMentor = await _userService.GetUserById(mentor.UserId);
                response.Data = GetMeetingScheduleResponse(newMeetingSchedule, project, group, userWithRoleMentor);
                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }

        [HttpPost("edit/{meetingScheduleIdStr}")]
        public async Task<IActionResult> EditMeeting(string meetingScheduleIdStr, [FromBody] MeetingScheduleModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<MeetingScheduleResponse> response = new ApiResponse<MeetingScheduleResponse>();
            try
            {
                if (string.IsNullOrEmpty(meetingScheduleIdStr) || !int.TryParse(meetingScheduleIdStr, out int meetingScheduleId))
                {
                    throw new Exception("Invalid Meeting schedule Id");
                }
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid User Id");
                }
                if (model.MeetingDate < DateTime.Now) throw new Exception("Cannot select date in past time!");
                if (model.Duration <= 0) throw new Exception("Duration cannot be negative or zero.");
                var meetingSchedule = _mapper.Map<MeetingSchedule>(model);
                var editedMeetingSchedule = await _meetingScheduleService.EditMeetingSchedule(meetingScheduleId, meetingSchedule);
                if (editedMeetingSchedule == null) throw new Exception("Edit meeting failed");
                var project = await _projectService.GetProjectWithTopicByGroupId(editedMeetingSchedule.GroupId);
                var group = await _groupService.GetGroupById(editedMeetingSchedule.GroupId);
                var userWithRoleMentor = await _userService.GetUserById(userId);
                response.Data = GetMeetingScheduleResponse(editedMeetingSchedule, project, group, userWithRoleMentor);
                var users = await _groupMemberService.GetUsersByGroupId(group.GroupId);
                foreach (var user in users)
                {
                    string formattedDate = editedMeetingSchedule.MeetingDate.ToString("hh:mm tt on MM/dd");
                    var newNotification = await _notificationService.CreateNotification(userId, user.UserId,
                            $"A meeting schedule with your group has been edit to {formattedDate}, please go to meeting schedule to view it.");
                    await _hubContext.Clients.User(user.UserId.ToString()).SendAsync("ReceiveNotification", newNotification);
                }
                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }

        [HttpDelete("delete/{meetingScheduleIdStr}")]
        public async Task<IActionResult> DeleteMeeting(string meetingScheduleIdStr)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                if (string.IsNullOrEmpty(meetingScheduleIdStr) || !int.TryParse(meetingScheduleIdStr, out int meetingScheduleId))
                {
                    throw new Exception("Invalid Meeting Schedule Id");
                }
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid User Id");
                }
                var meetingSchedule = await _meetingScheduleService.GetMeetingScheduleByMeetingScheduleId(meetingScheduleId);
                if (meetingSchedule == null) throw new Exception($"Meeting not found with id = {meetingScheduleId}");
                var result = await _meetingScheduleService.DeleteMeetingSchedule(meetingScheduleId);
                if (result)
                {
                    var users = await _groupMemberService.GetUsersByGroupId(meetingSchedule.GroupId);
                    foreach (var user in users)
                    {
                        string formattedDate = meetingSchedule.MeetingDate.ToString("hh:mm tt on MM/dd");
                        var newNotification = await _notificationService.CreateNotification(userId, user.UserId,
                                $"There was a meeting scheduled with your group at {formattedDate} that was deleted.");
                        await _hubContext.Clients.User(user.UserId.ToString()).SendAsync("ReceiveNotification", newNotification);
                    }
                    response.Data = "Meeting deleted successfully!";
                    return Ok(response);
                }
                throw new Exception("Delete meeting failed.");

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }

        private MeetingScheduleResponse GetMeetingScheduleResponse(MeetingSchedule meetingSchedule, Project? project, Group group, User user)
        {
            return new MeetingScheduleResponse
            {
                ScheduleId = meetingSchedule.ScheduleId,
                MeetingScheduleName = meetingSchedule.MeetingScheduleName,
                Content = meetingSchedule.Content,
                StartDate = meetingSchedule.MeetingDate,
                EndDate = meetingSchedule.MeetingDate.AddMinutes(meetingSchedule.Duration),
                TopicName = project?.Topic?.TopicName != null ? project.Topic.TopicName : "The group has no topic yet.",
                GroupName = group.GroupName,
                Location = meetingSchedule.Location,
                MentorName = user.FullName,
                Type = meetingSchedule.Type,
                Url = meetingSchedule.Url,
                IsDeleted = meetingSchedule.IsDeleted
            };
        }

    }
}
