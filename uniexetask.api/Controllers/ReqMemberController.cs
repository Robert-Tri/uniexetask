using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services;
using uniexetask.services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace uniexetask.api.Controllers
{
    [Route("api/reqMembers")]
    [ApiController]
    public class ReqMemberController : ControllerBase
    {
        private readonly IGroupMemberService _groupMemberService;
        private readonly IReqMemberService _reqMemberService;
        private readonly IMapper _mapper;

        public ReqMemberController(IReqMemberService reqMemberService, IMapper mapper, IGroupMemberService groupMemberService)
        {
            _reqMemberService = reqMemberService;
            _groupMemberService = groupMemberService;
            _mapper = mapper;
        }
   

        [HttpGet]
        public async Task<IActionResult> GetReqMembersList()
        {
            var reqMembersList = await _reqMemberService.GetAllReqMember();
            if (reqMembersList == null || !reqMembersList.Any())
            {
                return NotFound();
            }

            var responseData = reqMembersList
                .Where(reqMember => reqMember.Status == true)
                .Select(reqMember => new
                {
                    reqMember.RegMemberId,
                    reqMember.Description,
                    reqMember.Status,
                    GroupName = reqMember.Group.GroupName,
                    GroupId = reqMember.Group.GroupId,
                    LeaderName = reqMember.Group.GroupMembers
                        .Where(gm => gm.Role == "Leader")
                        .Select(gm => gm.Student.User.FullName)
                        .FirstOrDefault(),
                    LeaderAvatar = reqMember.Group.GroupMembers
                        .Where(gm => gm.Role == "Leader")
                        .Select(gm => gm.Student.User.Avatar)
                        .FirstOrDefault(),
                    SubjectCode = reqMember.Group.Subject.SubjectCode,
                    MemberCount = reqMember.Group.GroupMembers.Count()
                });

            var response = new ApiResponse<IEnumerable<object>>
            {
                Data = responseData
            };
            return Ok(response);
        }

        [Authorize(Roles = "Student")]
        [HttpGet("MyFindGroup")]
        public async Task<IActionResult> GetMyReqMembers()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var myReqMembers = await _groupMemberService.GetGroupMemberByUserId(userId);
            if (myReqMembers == null || myReqMembers.GroupId == 0)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorMessage = "User does not belong to any group." });
            }

            var reqMembersList = await _reqMemberService.GetAllReqMember();
            if (reqMembersList == null || !reqMembersList.Any())
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorMessage = "No member requests available." });
            }

            var responseData = reqMembersList
                .Where(reqMember => reqMember.Status == true && reqMember.GroupId == myReqMembers.GroupId)
                .Select(reqMember => new
                {
                    reqMember.RegMemberId,
                    reqMember.Description,
                    reqMember.Status,
                    GroupName = reqMember.Group.GroupName,
                    GroupId = reqMember.Group.GroupId,
                    LeaderName = reqMember.Group.GroupMembers
                        .Where(gm => gm.Role == "Leader")
                        .Select(gm => gm.Student.User.FullName)
                        .FirstOrDefault(),
                    LeaderAvatar = reqMember.Group.GroupMembers
                        .Where(gm => gm.Role == "Leader")
                        .Select(gm => gm.Student.User.Avatar)
                        .FirstOrDefault(),
                    SubjectCode = reqMember.Group.Subject.SubjectCode,
                    MemberCount = reqMember.Group.GroupMembers.Count()
                });

            var response = new ApiResponse<IEnumerable<object>>
            {
                Success = true,
                Data = responseData
            };

            return Ok(response);
        }


        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> CreateReqMember([FromBody] RegMemberFormModel reqMember)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var groupMember = await _groupMemberService.GetGroupMemberByUserId(userId);

                if (groupMember != null)
                {
                    reqMember.GroupId = groupMember.GroupId;
                    reqMember.Status = true;

                    var obj = _mapper.Map<RegMemberForm>(reqMember);
                    var isRegMemberCreated = await _reqMemberService.CreateReqMember(obj);

                    if (isRegMemberCreated)
                    {
                        var response = new ApiResponse<RegMemberFormModel>
                        {
                            Data = reqMember
                        };
                        return Ok(response);
                    }
                    else
                    {
                        return BadRequest("Không thể tạo yêu cầu thành viên.");
                    }
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

    }
}
