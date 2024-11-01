using AutoMapper;
using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
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
        private readonly IGroupService _groupService;
        private readonly INotificationService _notificationService;
        private readonly IStudentService _studentService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;

        public GroupMemberController(INotificationService notificationService, IStudentService studentService, IGroupService groupService, IGroupMemberService groupMemberService, IMapper mapper, IHubContext<NotificationHub> hubContext)
        {
            _notificationService = notificationService;
            _studentService = studentService;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _mapper = mapper;
            _hubContext = hubContext;
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

        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpPost("AddMemberToGroup")]
        public async Task<IActionResult> AddMemberToGroup([FromBody] AddGroupMemberModel member)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest("ID người dùng không hợp lệ.");
            }

            var student = await _studentService.GetStudentByCode(member.StudentCode);
            if (student == null)
            {
                return BadRequest(new { StudentCode = member.StudentCode, Success = false, Message = "Không tìm thấy sinh viên" });
            }

            bool studentExistsInGroup = await _groupMemberService.CheckIfStudentInGroup(student.StudentId);
            if (studentExistsInGroup)
            {
                return BadRequest(new { StudentCode = member.StudentCode, Success = false, Message = "Sinh viên đã có nhóm" });
            }

            var group = await _groupService.GetGroupById(member.GroupId);
            if (group == null)
            {
                return BadRequest("Không tìm thấy nhóm.");
            }

            var newNotification = await _notificationService.CreateGroupInvite(senderId: userId, receiverId: student.UserId, groupId: member.GroupId, groupName: group.GroupName);
            await _hubContext.Clients.User(student.UserId.ToString()).SendAsync("ReceiveNotification", newNotification);

            return Ok(new { Success = true, Message = "Đã gửi lời mời tham gia nhóm" });
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
            // Retrieve user ID from the claims
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Validate and parse user ID
            if (int.TryParse(userIdString, out int userId))
            {
                // Fetch GroupMember by userId
                var groupMember = await _groupMemberService.GetGroupMemberByUserId(userId);

                if (groupMember != null)
                {
                    // Prepare successful response
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

        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpPost("CreateGroupWithMember")]
        public async Task<IActionResult> CreateGroupWithMember([FromBody] CreateGroupWithMemberModel request)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            request.Group.HasMentor = false;
            request.Group.Status = nameof(GroupStatus.Initialized);
            

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest("ID người dùng không hợp lệ.");
            }

            var studentLeader = await _studentService.GetStudentByUserId(userId);
            bool isLeaderInGroup = await _groupMemberService.CheckIfStudentInGroup(studentLeader.StudentId);
            if (isLeaderInGroup)
            {
                return BadRequest("Leader đã có nhóm, không thể tạo nhóm mới.");
            }

            // Tạo nhóm
            var objGroup = _mapper.Map<uniexetask.core.Models.Group>(request.Group);
            var isGroupCreated = await _groupService.CreateGroup(objGroup);

            if (!isGroupCreated)
            {
                return BadRequest("Tạo nhóm thất bại");
            }

            var createdGroupId = objGroup.GroupId;
            var student = await _studentService.GetStudentByUserId(userId);

            if (student != null)
            {
                var leaderMember = new GroupMemberModel
                {
                    GroupId = createdGroupId,
                    StudentId = student.StudentId,
                    Role = nameof(GroupMemberRole.Leader)
                };

                var objLeader = _mapper.Map<GroupMember>(leaderMember);
                var isLeaderCreated = await _groupMemberService.AddMember(objLeader);

                if (!isLeaderCreated)
                {
                    return BadRequest("Không thể thêm Leader vào nhóm");
                }
            }
            else
            {
                return BadRequest("Không tìm thấy sinh viên với ID đã cho");
            }

            // Kiểm tra mã sinh viên trùng lặp
            var studentCodesHashSet = new HashSet<string>();
            var memberCreationResults = new List<object>(); // Để lưu kết quả tạo thành viên

            foreach (var studentCode in request.StudentCodes)
            {
                if (!studentCodesHashSet.Add(studentCode))
                {
                    memberCreationResults.Add(new { StudentCode = studentCode, Success = false, Message = "Mã sinh viên trùng, đã bỏ qua" });
                    continue;
                }

                var foundStudent = await _studentService.GetStudentByCode(studentCode);
                if (foundStudent == null)
                {
                    memberCreationResults.Add(new { StudentCode = studentCode, Success = false, Message = "Không tìm thấy sinh viên" });
                    continue;
                }

                bool studentExistsInGroup = await _groupMemberService.CheckIfStudentInGroup(foundStudent.StudentId);
                if (studentExistsInGroup)
                {
                    memberCreationResults.Add(new { StudentCode = studentCode, Success = false, Message = "Sinh viên đã có nhóm" });
                    continue;
                }

                var newNotification = await _notificationService.CreateGroupInvite(senderId: userId, receiverId: foundStudent.UserId, groupId: createdGroupId, groupName: request.Group.GroupName);
                await _hubContext.Clients.User(foundStudent.UserId.ToString()).SendAsync("ReceiveNotification", newNotification);
                memberCreationResults.Add(new { StudentCode = studentCode, Success = true, Message = "Đã gửi lời mời tham gia nhóm" });
            }

            return Ok(new { GroupId = createdGroupId, MemberResults = memberCreationResults });
        }




        [HttpGet("GetUsersByGroupId/{groupId}")]
        public async Task<IActionResult> GetUsersByGroupId(int groupId)
        {
            var users = await _groupMemberService.GetUsersByGroupId(groupId);

            if (users != null && users.Any())
            {
                return Ok(users); 
            }
            else
            {
                return NotFound("No members found in this group.");
            }
        }

        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpGet("GetUsersByUserId")]
        public async Task<IActionResult> GetUsersByUserId()
        {
            // Lấy user ID từ claims
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra xem user ID có tồn tại trong claims không
            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest("User ID not found.");
            }

            // Chuyển đổi user ID từ string sang integer
            if (!int.TryParse(userIdString, out int userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            // Lấy danh sách người dùng liên kết với nhóm của sinh viên
            var users = await _groupMemberService.GetUsersByUserId(userId);

            // Kiểm tra xem có người dùng nào được tìm thấy không
            if (users == null || !users.Any())
            {
                return NotFound("No members found in this group.");
            }

            // Duyệt qua danh sách và lấy userId, fullName của từng người dùng
            var userDetails = users.Select(u => new
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Major = u.Students.FirstOrDefault()?.Major,         // Lấy Major từ đối tượng Student
                StudentId = u.Students.FirstOrDefault()?.StudentId, // Lấy StudentId từ đối tượng Student
                StudentCode = u.Students.FirstOrDefault()?.StudentCode, // Lấy StudentCode từ đối tượng Student
                Role = u.Students.FirstOrDefault()?.GroupMembers.FirstOrDefault()?.Role, // Lấy Role từ GroupMembers
                GroupId = u.Students.FirstOrDefault()?.GroupMembers.FirstOrDefault()?.GroupId // Lấy Role từ GroupMembers
            }).ToList();

            // Trả về danh sách người dùng trong nhóm
            return Ok(userDetails);
        }




    }
}
