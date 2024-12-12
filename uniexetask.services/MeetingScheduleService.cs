using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Hubs;
using uniexetask.services.Interfaces;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;

namespace uniexetask.services
{
    public class MeetingScheduleService : IMeetingScheduleService
    {
        public IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITopicService _topicService;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        private readonly IMentorService _mentorService;
        private readonly IProjectService _projectService;
        private readonly IStudentService _studentService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public MeetingScheduleService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
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
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        public async Task<MeetingScheduleResponse> CreateMeetingSchedule(int userId, MeetingScheduleModel model)
        {
            if (model.MeetingDate < DateTime.Now) throw new Exception("Cannot create event in past time.");
            var mentor = await _mentorService.GetMentorWithGroupAsync(userId);
            if (mentor == null) throw new Exception("There is no mentor corresponding to the provided user.");
            model.MentorId = mentor.MentorId;
            var meetingSchedule = _mapper.Map<MeetingSchedule>(model);
            if (meetingSchedule == null) throw new Exception("Invalid model.");
            if (!mentor.Groups.Any(m => m.GroupId == meetingSchedule.GroupId)) throw new Exception("You are not the mentor of this group.");
            var group = await _groupService.GetGroupById(meetingSchedule.GroupId);
            if (group == null) throw new Exception("You are creating an meeting for a group that does not exist.");
            if (group.Status == nameof(GroupStatus.Initialized)) throw new Exception($"{group.GroupName} Group is in newly created state and has not been assigned a mentor yet.");
            var mentorExists = await _unitOfWork.Mentors.GetByIDAsync(meetingSchedule.MentorId);
            var groupExists = await _unitOfWork.Groups.GetByIDAsync(meetingSchedule.GroupId);

            if (mentorExists == null || groupExists == null)
            {
                throw new Exception("Mentor or group do not exist.");
            }

            var mentorInGroup = await _unitOfWork.Groups.GetMentorInGroup(groupExists.GroupId);
            if (mentorInGroup == null) throw new Exception($"There are no mentors in {groupExists.GroupName} group.");
            if (mentorInGroup.UserId != mentorExists.UserId) throw new Exception($"You are not a mentor of the {groupExists.GroupName} group.");
            await _unitOfWork.MeetingSchedules.InsertAsync(meetingSchedule);
            _unitOfWork.Save();

            var users = await _groupMemberService.GetUsersByGroupId(group.GroupId);
            foreach (var user in users)
            {
                string formattedDate = meetingSchedule.MeetingDate.ToString("hh:mm tt on MM/dd");
                var newNotification = await _notificationService.CreateNotification(userId, user.UserId,
                        $"A meeting schedule with your group has been created at <b>{formattedDate}</b>, please go to meeting schedule to view it.");
                await _hubContext.Clients.User(user.UserId.ToString()).SendAsync("ReceiveNotification", newNotification);
            }
            var project = await _projectService.GetProjectWithTopicByGroupId(meetingSchedule.GroupId);
            group = await _groupService.GetGroupById(meetingSchedule.GroupId);
            var userWithRoleMentor = await _userService.GetUserById(mentor.UserId);

            return GetMeetingScheduleResponse(meetingSchedule, project, group, userWithRoleMentor); ;
        }

        public async Task<bool> DeleteMeetingSchedule(int userId, int meetingScheduleId)
        {
            var meetingSchedule = await _unitOfWork.MeetingSchedules.GetByIDAsync(meetingScheduleId);
            if (meetingSchedule == null) throw new Exception("Meeting not found.");
            meetingSchedule.IsDeleted = true;
            _unitOfWork.MeetingSchedules.Update(meetingSchedule);
            _unitOfWork.Save();
            var users = await _groupMemberService.GetUsersByGroupId(meetingSchedule.GroupId);
            foreach (var user in users)
            {
                string formattedDate = meetingSchedule.MeetingDate.ToString("hh:mm tt on MM/dd");
                var newNotification = await _notificationService.CreateNotification(userId, user.UserId,
                        $"There was a meeting scheduled with your group at <b>{formattedDate}</b> that was deleted.");
                await _hubContext.Clients.User(user.UserId.ToString()).SendAsync("ReceiveNotification", newNotification);
            }
            return true;
        }

        public async Task<bool> EditMeetingSchedule(int userId, int meetingScheduleId, MeetingScheduleModel model)
        {
            if (model.MeetingDate < DateTime.Now) throw new Exception("Cannot select date in past time!");
            if (model.Duration <= 0) throw new Exception("Duration cannot be negative or zero.");
            var meetingScheduleEntity = _mapper.Map<MeetingSchedule>(model);
            if (meetingScheduleEntity.Type == nameof(MeetingScheduleType.Offline)) meetingScheduleEntity.Url = null;
            var meetingSchedule = await _unitOfWork.MeetingSchedules.GetByIDAsync(meetingScheduleId);
            if (meetingSchedule == null) throw new Exception("Meeting schedule not found.");
            meetingSchedule.Content = meetingScheduleEntity.Content;
            meetingSchedule.Duration = meetingScheduleEntity.Duration;
            meetingSchedule.Location = meetingScheduleEntity.Location;
            meetingSchedule.Type = meetingScheduleEntity.Type;
            meetingSchedule.Url = meetingScheduleEntity.Url;
            meetingSchedule.MeetingScheduleName = meetingScheduleEntity.MeetingScheduleName;
            meetingSchedule.MeetingDate = meetingScheduleEntity.MeetingDate;
            _unitOfWork.MeetingSchedules.Update(meetingSchedule);
            _unitOfWork.Save();
            var group = await _groupService.GetGroupById(meetingSchedule.GroupId);
            var users = await _groupMemberService.GetUsersByGroupId(group.GroupId);
            foreach (var user in users)
            {
                string formattedDate = meetingSchedule.MeetingDate.ToString("hh:mm tt on MM/dd");
                var newNotification = await _notificationService.CreateNotification(userId, user.UserId,
                        $"A meeting schedule with your group has been edit to <b>{formattedDate}</b>, please go to meeting schedule to view it.");
                await _hubContext.Clients.User(user.UserId.ToString()).SendAsync("ReceiveNotification", newNotification);
            }
            return true;
        }

        public async Task<MeetingSchedule?> GetMeetingScheduleByMeetingScheduleId(int meetingScheduleId)
        {
            return await _unitOfWork.MeetingSchedules.GetByIDAsync(meetingScheduleId);
        }

        public async Task<IEnumerable<MeetingSchedule>> GetMeetingSchedulesByGroupId(int groupId)
        {
            return await _unitOfWork.MeetingSchedules.GetMeetingSchedulesByGroupId(groupId);
        }

        public async Task<IEnumerable<MeetingScheduleResponse>> GetMeetingSchedules(int userId ,string role)
        {
            List<MeetingScheduleResponse> list = new List<MeetingScheduleResponse>();
            if (role == nameof(EnumRole.Mentor))
            {
                var mentorWithGroups = await _mentorService.GetMentorWithGroupAsync(userId);
                if (mentorWithGroups == null) throw new Exception("There is no mentor with the provided user id.");
                foreach (var group in mentorWithGroups.Groups)
                {
                    if (group.IsDeleted) continue;
                    var project = await _projectService.GetProjectWithTopicByGroupId(group.GroupId);
                    var meetingSchedules = await _unitOfWork.MeetingSchedules.GetMeetingSchedulesByGroupId(group.GroupId);
                    if (meetingSchedules == null) throw new Exception("Load data failed.");
                    foreach (var meeting in meetingSchedules)
                    {
                        if (meeting.MentorId != mentorWithGroups.MentorId) continue;
                        var user = await _userService.GetUserById(mentorWithGroups.UserId);
                        if (user == null) throw new Exception("User not found from mentor.");
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
                    var meetingSchedules = await _unitOfWork.MeetingSchedules.GetMeetingSchedulesByGroupId(group.GroupId);
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
            return list;
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
