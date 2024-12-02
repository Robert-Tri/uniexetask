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
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private readonly IMentorService _mentorService;

        public ReqMemberController(IReqMemberService reqMemberService, IMentorService mentorService, IStudentService studentService, IMapper mapper, IGroupMemberService groupMemberService)
        {
            _mentorService = mentorService;
            _reqMemberService = reqMemberService;
            _groupMemberService = groupMemberService;
            _studentService = studentService;
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
                        .Where(gm => gm.Role == nameof(GroupMemberRole.Leader))
                        .Select(gm => gm.Student.User.FullName)
                        .FirstOrDefault(),
                    LeaderId = reqMember.Group.GroupMembers
                        .Where(gm => gm.Role == nameof(GroupMemberRole.Leader))
                        .Select(gm => gm.Student.User.UserId)
                        .FirstOrDefault(),
                    LeaderAvatar = reqMember.Group.GroupMembers
                        .Where(gm => gm.Role == nameof(GroupMemberRole.Leader))
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
            if (reqMembersList == null )
            {
                reqMembersList = new List<RegMemberForm>(); 
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
                        .Where(gm => gm.Role == nameof(GroupMemberRole.Leader))
                        .Select(gm => gm.Student.User.FullName)
                        .FirstOrDefault(),
                    LeaderAvatar = reqMember.Group.GroupMembers
                        .Where(gm => gm.Role == nameof(GroupMemberRole.Leader))
                        .Select(gm => gm.Student.User.Avatar)
                        .FirstOrDefault(),
                    SubjectCode = reqMember.Group.Subject.SubjectCode,
                    MemberCount = reqMember.Group.GroupMembers.Count(),
                    Role = reqMember.Group.GroupMembers
                        .Where(gm => gm.Student.UserId == userId) // Lấy role của thành viên hiện tại
                        .Select(gm => gm.Role)
                        .FirstOrDefault()
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
                var role = await _groupMemberService.GetRoleByUserId(userId);

                if (role != "Leader")
                {
                    return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You are not a leader to perform this operation." });
                }
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

        [Authorize(Roles = "Student")]
        [HttpPut]
        public async Task<IActionResult> UpdateReqMember([FromBody] UpdateRegMemberModel reqMember)
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

            var reqNew = await _reqMemberService.GetReqMemberById(reqMember.RegMemberId);

            if (reqNew == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorMessage = "Không tìm thấy yêu cầu với ID đã cho." });
            }

            reqNew.Description = reqMember.Description;
            var isReqUpdated = await _reqMemberService.UpdateReqMember(reqNew);

            ApiResponse<object> response = new ApiResponse<object>
            {
                Data = new
                {
                    Description = reqNew.Description
                }
            };

            if (isReqUpdated)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Không thể cập nhật Description.");
            }
        }


        [Authorize(Roles = "Student")]
        [HttpPut("DeleteReq")]
        public async Task<IActionResult> DeleteReqMember([FromBody] int reqMemberId)
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
            var reqNew = await _reqMemberService.GetReqMemberById(reqMemberId);
            reqNew.Status = false;
            var isReqUpdated = await _reqMemberService.UpdateReqMember(reqNew);
            ApiResponse<object> response = new ApiResponse<object>
            {
                Data = new
                {
                    Description = reqNew.Description
                }
            };

            if (isReqUpdated)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Không thể cập nhật Description.");
            }
        }

    }
}
