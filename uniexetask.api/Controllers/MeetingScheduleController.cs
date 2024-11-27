using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        public MeetingScheduleController(
            IMapper mapper,
            IMeetingScheduleService meetingScheduleService, 
            ITopicService topicService, 
            IGroupService groupServic, 
            IUserService userService, 
            IMentorService mentorService,
            IProjectService projectService,
            IStudentService studentService,
            IGroupMemberService groupMemberService)
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
        }

        [HttpGet("prepare-data")]
        public async Task<IActionResult> GetPrepareData()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            ApiResponse<IEnumerable<MeetingSchedulePrepareDataResponse>> response = new ApiResponse<IEnumerable<MeetingSchedulePrepareDataResponse>>();
            List<MeetingSchedulePrepareDataResponse> list = new List<MeetingSchedulePrepareDataResponse>();
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
                        if (project == null) 
                        {
                            System.Diagnostics.Debug.WriteLine($"Project not found with group id = {group.GroupId}");
                            continue;
                        } 
                        if (project.Topic == null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Topic not found with project id = {project.ProjectId}");
                            continue;
                        }
                        list.Add(new MeetingSchedulePrepareDataResponse
                        {
                            GroupId = group.GroupId,
                            GroupName = group.GroupName,
                            TopicCode = project.Topic.TopicCode,
                            TopicName = project.Topic.TopicName,
                            TopicDescription = project.Topic.Description
                        });

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
                        if (project == null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Project not found with group id = {group.GroupId}");
                        }
                        else
                        {
                            if (project.Topic == null)
                            {
                                System.Diagnostics.Debug.WriteLine($"Topic not found with project id = {project.ProjectId}");
                            }
                            else 
                            {
                                list.Add(new MeetingSchedulePrepareDataResponse
                                {
                                    GroupId = group.GroupId,
                                    GroupName = group.GroupName,
                                    TopicCode = project.Topic.TopicCode,
                                    TopicName = project.Topic.TopicName,
                                    TopicDescription = project.Topic.Description
                                });
                            }
                            
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
                return BadRequest(response);
            }
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetMeetingSchedules(int groupId)
        {
            ApiResponse<IEnumerable<MeetingScheduleModel>> response = new ApiResponse<IEnumerable<MeetingScheduleModel>>();
            List<MeetingScheduleModel> list = new List<MeetingScheduleModel>();
            try
            {
                var meetingSchedules = await _meetingScheduleService.GetMeetingSchedulesByGroupId(groupId);
                foreach (var schedule in meetingSchedules) 
                {
                    list.Add(_mapper.Map<MeetingScheduleModel>(schedule));
                }
                response.Data = list;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("details/{meetingScheduleId}")]
        public async Task<IActionResult> GetMeetingScheduleDetais(int meetingScheduleId)
        {
            ApiResponse<MeetingScheduleModel> response = new ApiResponse<MeetingScheduleModel>();
            try
            {
                var meetingSchedule = await _meetingScheduleService.GetMeetingScheduleByMeetingScheduleId(meetingScheduleId);
                if (meetingSchedule == null) throw new Exception("Get data failed.");
                var meetingScheduleModel = _mapper.Map<MeetingScheduleModel>(meetingSchedule);
                response.Data = meetingScheduleModel;
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
