using AutoMapper;
using AutoMapper.Execution;
using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using uniexetask.api.Hubs;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services;
using uniexetask.services.Interfaces;
using static Google.Apis.Requests.BatchRequest;

namespace uniexetask.api.Controllers
{
    [Route("api/groupMember")]
    [ApiController]
    public class GroupMemberController : ControllerBase
    {
        private IConfigSystemService _configSystemService;
        private readonly IMentorService _mentorService;
        private readonly IGroupService _groupService;
        private readonly ICampusService _campusService;
        private readonly INotificationService _notificationService;
        private readonly IChatGroupService _chatGroupService;
        private readonly IStudentService _studentService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;

        public GroupMemberController(
            IConfigSystemService configSystemService,
            ICampusService campusService, 
            IMentorService mentorService, 
            INotificationService notificationService, 
            IStudentService studentService, 
            IGroupService groupService, 
            IGroupMemberService groupMemberService, 
            IMapper mapper,
            IHubContext<NotificationHub> hubContext,
            IChatGroupService chatGroupService)
        {
            _configSystemService = configSystemService;
            _mentorService = mentorService;
            _notificationService = notificationService;
            _studentService = studentService;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _campusService = campusService;
            _mapper = mapper;
            _hubContext = hubContext;
            _chatGroupService = chatGroupService;
        }



        [HttpGet]
        public async Task<IActionResult> GetGroupMemberList()
        {
            var groupMemberList = await _groupMemberService.GetAllGroupMember();
            if (groupMemberList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<GroupMember>> response = new ApiResponse<IEnumerable<GroupMember>>();
            response.Data = groupMemberList;
            return Ok(response);
        }
        [HttpGet("getmembergroupbyprojectid")]
        public async Task<IActionResult> GetMemberGroupByProjectId(int projectId)
        {
            var groupMember = await _groupMemberService.GetGroupMemberByProjectId(projectId);
            if (groupMember == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<GroupMember>> response = new ApiResponse<IEnumerable<GroupMember>>();
            response.Data = groupMember;
            return Ok(response);
        }

        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpPost("AddMemberToGroup")]
        public async Task<IActionResult> AddMemberToGroup([FromBody] AddGroupMemberModel member)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var studentLeader = await _studentService.GetStudentByUserId(userId);

            var role = await _groupMemberService.GetRoleByUserId(userId);

            if (role != "Leader")
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You are not a leader to perform this operation." });
            }

            var group = await _groupService.GetGroupById(member.GroupId);

            if (group.Status != nameof(GroupStatus.Initialized))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Group is eligible, you cannot add members." });
            }

            var student = await _studentService.GetStudentByCode(member.StudentCode);
            if (student == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Data = new { StudentCode = member.StudentCode },
                    ErrorMessage = $"Student with code {member.StudentCode} not found."
                });
            }

            var campusLeader = await _campusService.GetCampusByStudentCode(studentLeader.StudentCode);
            var campusUser = await _campusService.GetCampusByStudentCode(member.StudentCode);

            if (campusLeader != campusUser)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You cannot add students from another campus." });
            }

            bool studentExistsInGroup = await _groupMemberService.CheckIfStudentInGroup(student.StudentId);

            var users = await _groupMemberService.GetUsersByUserId(userId);
            var subject = await _studentService.GetStudentById(studentLeader.StudentId);

            if (subject.SubjectId != student.SubjectId)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You cannot add members from a different subject." });
            }

            var maxMemberExe101 = (await _configSystemService.GetConfigSystems())
           .FirstOrDefault(config => config.ConfigName == "MAX_MEMBER_EXE101");
            var maxMemberExe201 = (await _configSystemService.GetConfigSystems())
            .FirstOrDefault(config => config.ConfigName == "MAX_MEMBER_EXE201");

            if (subject.SubjectId == 1 && users.Count >= maxMemberExe101.Number)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You already have enough members for EXE101." });
            }

            if (subject.SubjectId == 2 && users.Count >= maxMemberExe201.Number)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You already have enough members for EXE201." });
            }

            if (studentExistsInGroup)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Data = new { StudentCode = member.StudentCode },
                    ErrorMessage = $"Student with code {member.StudentCode} is already in a group."
                });
            }

            
            if (group == null)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = $"Group with ID {member.GroupId} not found." });
            }

            var newNotification = await _notificationService.CreateGroupInvite(senderId: userId, receiverId: student.UserId, groupId: member.GroupId, groupName: group.GroupName);
            await _hubContext.Clients.User(student.UserId.ToString()).SendAsync("ReceiveNotification", newNotification);

            var response = new ApiResponse<object>
            {
                Data = new { Message = $"Invitation to join the group has been sent to student with code {member.StudentCode}." }
            };
            return Ok(response);
        }

        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpPost("CreateGroupWithMember")]
        public async Task<IActionResult> CreateGroupWithMember([FromBody] CreateGroupWithMemberModel request)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var studentLeader = await _studentService.GetStudentByUserId(userId);
            if (studentLeader == null)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Student information not found." });
            }

            bool isLeaderInGroup = await _groupMemberService.CheckIfStudentInGroup(studentLeader.StudentId);
            if (isLeaderInGroup)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You already have a group and cannot create a new one." });
            }

            var groups = await _groupService.GetGroupAndSubject();
            foreach (var group in groups)
            {
                if (request.Group.GroupName == group.GroupName)
                {
                    return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = $"Group with name {request.Group.GroupName} already exists." });
                }
            }

            var usersCount = request.StudentCodes.Count;

            var maxMemberExe101 = (await _configSystemService.GetConfigSystems())
            .FirstOrDefault(config => config.ConfigName == "MAX_MEMBER_EXE101");
            var maxMemberExe201 = (await _configSystemService.GetConfigSystems())
            .FirstOrDefault(config => config.ConfigName == "MAX_MEMBER_EXE201");

            if (studentLeader.SubjectId == 1 && usersCount >= maxMemberExe101.Number)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "The limit is 6 people." });
            }

            if (studentLeader.SubjectId == 2 && usersCount >= maxMemberExe201.Number)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "The limit is 8 people." });
            }

            var studentCodesHashSet = new HashSet<string>();
            foreach (var studentCode in request.StudentCodes)
            {
                var campusLeader = await _campusService.GetCampusByStudentCode(studentLeader.StudentCode);
                var campusUser = await _campusService.GetCampusByStudentCode(studentCode);

                if (campusLeader != campusUser)
                {
                    return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You cannot add students from another campus." });
                }

                var student = await _studentService.GetStudentByCode(studentCode);
                if (student == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        ErrorMessage = $"Student with code {studentCode} not found."
                    });
                }

                if (studentLeader.SubjectId != student.SubjectId)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        ErrorMessage = $"Student with code {studentCode} is not in the same subject."
                    });
                }

                if (!studentCodesHashSet.Add(studentCode))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        ErrorMessage = $"Student code {studentCode} is duplicated, please fix it."
                    });
                }

                bool studentExistsInGroup = await _groupMemberService.CheckIfStudentInGroup(student.StudentId);
                if (studentExistsInGroup)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        ErrorMessage = $"Student with code {studentCode} is already in a group."
                    });
                }
            }

            // All conditions are valid, proceed with group creation
            request.Group.SubjectId = studentLeader.SubjectId;
            request.Group.HasMentor = false;
            request.Group.IsCurrentPeriod = true;
            request.Group.IsDeleted = false;
            request.Group.Status = nameof(GroupStatus.Initialized);


            var objGroup = _mapper.Map<uniexetask.core.Models.Group>(request.Group);
            var isGroupCreated = await _groupService.CreateGroup(objGroup);

            if (!isGroupCreated)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Group creation failed." });
            }

            var createdGroupId = objGroup.GroupId;

            // Add leader to the group
            var leaderMember = new GroupMemberModel
            {
                GroupId = createdGroupId,
                StudentId = studentLeader.StudentId,
                Role = nameof(GroupMemberRole.Leader)
            };

            var objLeader = _mapper.Map<GroupMember>(leaderMember);
            var isLeaderCreated = await _groupMemberService.AddMember(objLeader);

            if (!isLeaderCreated)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Failed to add the Leader to the group." });
            }

            // Send group invite notifications to other members
            var memberCreationResults = new List<object>();
            foreach (var studentCode in request.StudentCodes)
            {
                var student = await _studentService.GetStudentByCode(studentCode);

                var newNotification = await _notificationService.CreateGroupInvite(senderId: userId, receiverId: student.UserId, groupId: createdGroupId, groupName: request.Group.GroupName);
                await _hubContext.Clients.User(student.UserId.ToString()).SendAsync("ReceiveNotification", newNotification);
                memberCreationResults.Add(new { StudentCode = studentCode, Success = true, Message = $"Invitation to join the group has been sent to student with code {studentCode}" });
            }

            var chatGroupCreated = await _chatGroupService.CreateChatGroupForGroup(objGroup, userId);
            if (!chatGroupCreated) return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Chat group can not created." });

            var response = new ApiResponse<object>
            {
                Data = new
                {
                    GroupId = createdGroupId,
                    MemberResults = memberCreationResults
                }
            };
            return Ok(response);
        }




        [HttpPost]
        public async Task<IActionResult> CreateGroupMember([FromBody] GroupMemberModel groupMember)
        {

            var obj = _mapper.Map<GroupMember>(groupMember);
            var isGroupCreated = await _groupMemberService.CreateGroupMember(obj);

            if (isGroupCreated)
            {
                return Ok(isGroupCreated);
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpGet("GetGroupMemberByUserID")]
        public async Task<IActionResult> GetGroupMemberByUserID()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdString, out int userId))
            {
                var groupMember = await _groupMemberService.GetGroupMemberByUserId(userId);

                if (groupMember != null)
                {
                    var response = new ApiResponse<GroupMember>
                    {
                        Data = groupMember
                    };
                    return Ok(response);
                }
                else
                {
                    return NotFound("Không tìm thấy group với ID đã cho.");
                }
            }
            else
            {
                return BadRequest("ID người dùng không hợp lệ.");
            }
        }

        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpGet("GetRoleByUserId")]
        public async Task<IActionResult> GetRoleByUserId()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdString, out int userId))
            {
                var role = await _groupMemberService.GetRoleByUserId(userId);

                if (role != null)
                {
                    // Chuẩn bị phản hồi thành công với Role
                    var response = new ApiResponse<string>
                    {
                        Data = role
                    };
                    return Ok(response);
                }
                else
                {
                    return NotFound("Không tìm thấy vai trò của thành viên với userId đã cho.");
                }
            }
            else
            {
                return BadRequest("ID người dùng không hợp lệ.");
            }
        }


        [HttpGet("GetTeammateByUserID/{userId}")]
        public async Task<IActionResult> GetTeammateByUserID(int userId)
        {
            var student = await _studentService.GetStudentByUserId(userId);
            if (student == null)
            {
                return NotFound();
            }
            var groupMember = await _groupMemberService.GetUsersByUserId(userId);

            if (groupMember != null)
            {
                // Prepare successful response
                var response = new ApiResponse<List<User>>
                {
                    Data = groupMember
                };
                return Ok(response);
            }
            else
            {
                return NotFound("Không tìm thấy group với ID đã cho.");
            }
        }




        [HttpGet("GetUsersByGroupId/{groupId}")]
        public async Task<IActionResult> GetUsersByGroupId(int groupId)
        {
            var users = await _groupMemberService.GetUsersByGroupId(groupId);

            var mentor = await _mentorService.GetMentorNameByGroupId(groupId);

            if (users != null && users.Any())
            {
                var groupMemberDetails = users.Select(user => new GroupMemberDetailsModel
                {
                    userId = user.UserId,
                    fullName = user.FullName,
                    mentorName = mentor,
                    email = user.Email,
                    phone = user.Phone ?? "N/A",
                    major = user.Students.FirstOrDefault()?.Major ?? "N/A",
                    studentCode = user.Students.FirstOrDefault()?.StudentCode ?? "N/A",
                    role = user.Students.FirstOrDefault()?.GroupMembers
                                .FirstOrDefault(gm => gm.GroupId == groupId)?.Role ?? "N/A"
                }).ToList();

                return Ok(groupMemberDetails);
            }
            else
            {
                return NotFound("No members found in this group.");
            }
        }


        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpGet("MyGroup")]
        public async Task<IActionResult> GetUsersByUserId()
        {
            ApiResponse<IEnumerable<TeammateModel>> response = new ApiResponse<IEnumerable<TeammateModel>>();
            try
            {
                var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdString))
                {
                    return BadRequest("User ID not found.");
                }

                if (!int.TryParse(userIdString, out int userId))
                {
                    return BadRequest("Invalid User ID format.");
                }

                var users = await _groupMemberService.GetUsersByUserId(userId);

                if (users == null || !users.Any())
                {
                    return NotFound("No members found in this group.");
                }

                var group = await _groupService.GetGroupByUserId(userId);

                // Khai báo biến mentorName
                string mentorName = null;

                if (group.HasMentor == true)
                {
                    mentorName = await _mentorService.GetMentorNameByGroupId(group.GroupId);
                }

                // Kiểm tra nếu mentorName là null thì gán giá trị mặc định
                if (string.IsNullOrEmpty(mentorName))
                {
                    mentorName = "The group has no Mentor";
                }

                var userDetails = users.Select(u => new TeammateModel
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Major = u.Students.FirstOrDefault()?.Major,
                    StudentId = u.Students.FirstOrDefault()?.StudentId,
                    StudentCode = u.Students.FirstOrDefault()?.StudentCode,
                    SubjectName = u.Students.FirstOrDefault()?.Subject?.SubjectName,
                    Role = u.Students.FirstOrDefault()?.GroupMembers.FirstOrDefault()?.Role,
                    GroupId = u.Students.FirstOrDefault()?.GroupMembers.FirstOrDefault()?.GroupId,
                    GroupName = u.Students.FirstOrDefault()?.GroupMembers.FirstOrDefault()?.Group?.GroupName,
                    MentorName = mentorName
                }).ToList();

                response.Data = userDetails;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }


        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpDelete("DeleteGroup")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var role = await _groupMemberService.GetRoleByUserId(userId);

            if (role != "Leader")
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You are not a leader to perform this operation." });
            }

            var group = await _groupService.GetGroupById(groupId);
            if (group.Status != nameof(GroupStatus.Initialized))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Group is eligible, you cannot delete." });
            }

            var deleteStatus = await _groupService.DeleteGroup(groupId);
            if (deleteStatus != true) 
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    ErrorMessage = "Fail to delete group ."
                });
            }

            bool isDeleted = await _groupMemberService.DeleteGroupMember(groupId);

            if (isDeleted)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = new { Message = "Group successfully deleted." }
                });
            }
            else
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    ErrorMessage = "Group delete fail."
                });
            }
        }

        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpDelete("DeleteMember")]
        public async Task<IActionResult> DeleteMember([FromBody] DeleteGroupMemberModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var role = await _groupMemberService.GetRoleByUserId(userId);

            if (role != "Leader")
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You are not a leader to perform this operation." });
            }

            var checkLeader = await _groupMemberService.GetGroupByStudentId(model.StudentId);
            if (checkLeader.Role == "Leader")
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    ErrorMessage = "You can not delete yourself ."
                });
            }

            bool isDeleted = await _groupMemberService.DeleteMember(model.GroupId, model.StudentId);

            if (isDeleted)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = new { Message = "Member successfully deleted." }
                });
            }
            else
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    ErrorMessage = "Member not found in the group."
                });
            }
        }
    }
}
