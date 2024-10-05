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
        public IGroupMemberService _groupMemberService;
        private readonly IMapper _mapper;

        public GroupMemberController(IGroupService groupService, IGroupMemberService groupMemberService, IMapper mapper)
        {
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
            
            var objGroup = _mapper.Map<Group>(request.Group);
            var isGroupCreated = await _groupService.CreateGroup(objGroup);

            if (!isGroupCreated)
            {
                return BadRequest("Failed to create group");
            }

            
            var createdGroupId = objGroup.GroupId;

            
            var member = request.Member;
            member.GroupId = createdGroupId; 
            if (string.IsNullOrEmpty(member.Role) || member.Role != "Member")
            {
                member.Role = "Member";
            }

            var objMember = _mapper.Map<GroupMember>(member);
            var isUserCreated = await _groupMemberService.AddMember(objMember);

            if (isUserCreated)
            {
                return Ok(new { GroupId = createdGroupId, MemberCreated = isUserCreated });
            }
            else
            {
                return BadRequest("Failed to add member to group");
            }
        }



    }
}
