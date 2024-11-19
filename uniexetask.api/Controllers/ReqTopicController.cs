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

namespace uniexetask.api.Controllers
{
    [Route("api/reqTopic")]
    [ApiController]
    public class ReqTopicController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IReqTopicService _reqTopicService;
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;

        public ReqTopicController(IReqTopicService reqTopicService, IMapper mapper, IGroupService groupService, IGroupMemberService groupMemberService)
        {
            _reqTopicService = reqTopicService;
            _mapper = mapper;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
        }   

        [HttpGet]
        public async Task<IActionResult> GetReqTopicList()
        {
            var reqTopicList = await _reqTopicService.GetAllReqTopic();
            if (reqTopicList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<RegTopicForm>> response = new ApiResponse<IEnumerable<RegTopicForm>>();
            response.Data = reqTopicList;
            return Ok(response);
        }

        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpPost]
        public async Task<IActionResult> CreateReqTopic([FromBody] ReqTopicModel reqTopic)
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

            var topicCodeMax = await _reqTopicService.GetMaxTopicCode();
            int nextCodeNumber = 1;
            if (!string.IsNullOrEmpty(topicCodeMax) && topicCodeMax.Length > 2)
            {
                var numericPart = topicCodeMax.Substring(2);
                if (int.TryParse(numericPart, out int currentMax))
                {
                    nextCodeNumber = currentMax + 1;
                }
            }

            reqTopic.TopicCode = $"TP{nextCodeNumber:D3}";

            var groupMember = await _groupMemberService.GetGroupMemberByUserId(userId);
            reqTopic.GroupId = groupMember.GroupId;
            reqTopic.Status = true;

            var obj = _mapper.Map<RegTopicForm>(reqTopic);
            var isTopicCreated = await _reqTopicService.CreateReqTopic(obj);

            if (isTopicCreated)
            {
                var response = new ApiResponse<object>
                {
                    Data = new { Message = "Topic created successfully!" }
                };
                return Ok(response);
            }
            else
            {
                return BadRequest("Lỗi Tạo.");
            }
        }


        [Authorize(Roles = "Student")]
        [HttpGet("MyTopic")]
        public async Task<IActionResult> GetMyReqTopics()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var groupMember = await _groupMemberService.GetGroupMemberByUserId(userId);
            if (groupMember == null || groupMember.GroupId == 0)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorMessage = "User does not belong to any group." });
            }

            var reqTopicList = await _reqTopicService.GetAllReqTopic();
            if (reqTopicList == null || !reqTopicList.Any())
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorMessage = "No topics available." });
            }

            var responseData = reqTopicList
                .Where(reqTopic => reqTopic.Status == true && reqTopic.GroupId == groupMember.GroupId)
                .Select(reqTopic => new
                {
                    reqTopic.RegTopicId,
                    reqTopic.GroupId,
                    reqTopic.TopicCode,
                    reqTopic.TopicName,
                    reqTopic.Description,
                    reqTopic.Status,
                    GroupName = reqTopic.Group.GroupName,
                    SubjectCode = reqTopic.Group.Subject.SubjectCode
                });

            var response = new ApiResponse<IEnumerable<object>>
            {
                Success = true,
                Data = responseData
            };

            return Ok(response);
        }


    }
}
