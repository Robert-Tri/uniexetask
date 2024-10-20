using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/groupMember")]
    [ApiController]
    public class GroupMemberController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IStudentService _studentService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IMapper _mapper;

        public GroupMemberController(IStudentService studentService, IGroupService groupService, IGroupMemberService groupMemberService, IMapper mapper)
        {
            _studentService = studentService;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _mapper = mapper;
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

        [HttpPost]
        public async Task<IActionResult> AddMemberToGroup([FromBody] GroupMemberModel member)
        {
            // Kiểm tra nếu Role trống hoặc null thì gán giá trị mặc định là "Member"
            if (string.IsNullOrEmpty(member.Role) || member.Role != "Member")
            {
                member.Role = "Member";
            }

            var obj = _mapper.Map<GroupMember>(member);
            var isUserCreated = await _groupMemberService.AddMember(obj);

            if (isUserCreated)
            {
                return Ok(isUserCreated);
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPost("CreateGroupWithMember")]
        public async Task<IActionResult> CreateGroupWithMember([FromBody] CreateGroupWithMemberModel request)
        {
            // Lấy userId từ claim
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Đặt mặc định HasMentor là false và Status là "Initialized"
            request.Group.HasMentor = false;
            request.Group.Status = "Initialized";

            // Tạo nhóm
            var objGroup = _mapper.Map<Group>(request.Group);
            var isGroupCreated = await _groupService.CreateGroup(objGroup);

            if (!isGroupCreated)
            {
                return BadRequest("Tạo nhóm thất bại");
            }

            var createdGroupId = objGroup.GroupId;

            if (!string.IsNullOrEmpty(userIdString))
            {
                if (int.TryParse(userIdString, out int userId))
                {
                    var student = await _studentService.GetStudentByUserId(userId);

                    if (student != null)
                    {
                        var leaderMember = new GroupMemberModel
                        {
                            GroupId = createdGroupId,
                            StudentId = student.StudentId,
                            Role = "Leader"
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
                }
                else
                {
                    return BadRequest("ID người dùng không hợp lệ.");
                }
            }
            else
            {
                return BadRequest("ID người dùng không được để trống.");
            }

            // Kiểm tra mã sinh viên trùng lặp
            var studentCodesHashSet = new HashSet<string>();
            var memberCreationResults = new List<object>(); // Để lưu kết quả tạo thành viên

            // Thêm các sinh viên vào nhóm
            foreach (var studentCode in request.StudentCodes)
            {
                if (!studentCodesHashSet.Add(studentCode))
                {
                    memberCreationResults.Add(new { StudentCode = studentCode, Success = false, Message = "Mã sinh viên trùng, đã bỏ qua" });
                    continue;
                }

                var student = await _studentService.GetStudentByCode(studentCode);
                if (student == null)
                {
                    memberCreationResults.Add(new { StudentCode = studentCode, Success = false, Message = "Không tìm thấy sinh viên" });
                    continue;
                }

                // Kiểm tra xem sinh viên đã có trong nhóm chưa
                bool studentExistsInGroup = await _groupMemberService.CheckIfStudentInGroup(student.StudentId);
                if (studentExistsInGroup)
                {
                    memberCreationResults.Add(new { StudentCode = studentCode, Success = false, Message = "Sinh viên đã có nhóm" });
                    continue;
                }

                var member = new GroupMemberModel
                {
                    GroupId = createdGroupId,
                    StudentId = student.StudentId,
                    Role = "Member"
                };

                var objMember = _mapper.Map<GroupMember>(member);
                var isUserCreated = await _groupMemberService.AddMember(objMember);

                memberCreationResults.Add(new
                {
                    StudentCode = studentCode,
                    Success = isUserCreated,
                    Message = isUserCreated ? "Thành viên đã được thêm thành công" : "Thêm thành viên thất bại"
                });
            }

            return Ok(new { GroupId = createdGroupId, MemberResults = memberCreationResults });
        }


        [HttpGet("GetUsersByGroupId/{groupId}")]
        public async Task<IActionResult> GetUsersByGroupId(int groupId)
        {
            var users = await _groupMemberService.GetUsersByGroupId(groupId);

            if (users != null && users.Any())
            {
                return Ok(users); // Trả về danh sách đầy đủ thông tin User
            }
            else
            {
                return NotFound("No members found in this group.");
            }
        }

    }
}
