using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private IGroupService _groupService;
        private IStudentService _studentService;
        public IGroupMemberService _groupMemberService;
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

        [HttpPost("CreateGroupWithMember")]
        public async Task<IActionResult> CreateGroupWithMember([FromBody] CreateGroupWithMemberModel request)
        {
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

            // Thêm nhiều sinh viên vào nhóm
            var memberCreationResults = new List<object>(); // Để lưu kết quả tạo thành viên
            foreach (var studentCode in request.StudentCodes)
            {
                var student = await _studentService.GetStudentByCode(studentCode);
                if (student == null)
                {
                    memberCreationResults.Add(new { StudentCode = studentCode, Success = false, Message = "Không tìm thấy sinh viên" });
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
