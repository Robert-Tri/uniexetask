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
    [Route("api/chat-groups")]
    [ApiController]
    public class ChatGroupController : ControllerBase
    {
        private readonly IChatGroupService _chatGroupService;
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;

        public ChatGroupController(IChatGroupService chatGroupService, IUserService userService, IGroupService groupService, IStudentService studentService, IMapper mapper)
        {
            _chatGroupService = chatGroupService;
            _userService = userService;
            _groupService = groupService;
            _studentService = studentService;
            _mapper = mapper;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetChatGroupByUser(int chatGroupIndex = 0, int limit = 5, string keyword = "")
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<IEnumerable<ChatGroupResponse>> response = new ApiResponse<IEnumerable<ChatGroupResponse>>();
            List<ChatGroupResponse> list = new List<ChatGroupResponse>();
            try
            {
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid User Id");
                }
                var chatgroups = await _chatGroupService.GetChatGroupByUserId(userId, chatGroupIndex, limit, keyword);
                if (chatgroups == null)
                {
                    throw new Exception("Chat group not found");
                }
                foreach (var chatgroup in chatgroups)
                {
                    Student? student = null;
                    if (chatgroup.Type == nameof(ChatGroupType.Personal))
                    {
                        var chatGroupWithUsers = await _chatGroupService.GetChatGroupWithUsersByChatGroupId(chatgroup.ChatGroupId);
                        if (chatGroupWithUsers != null && chatGroupWithUsers.Users != null && chatGroupWithUsers.Users.Count > 0)
                        {
                            foreach (var user in chatGroupWithUsers.Users)
                            {
                                if (user.UserId == userId) continue;
                                student = await _studentService.GetStudentByUserId(user.UserId);
                            }
                        }
                    }
                    var latestMessage = await _chatGroupService.GetLatestMessageInChatGroup(chatgroup.ChatGroupId);
                    list.Add(new ChatGroupResponse
                    {
                        ChatGroup = chatgroup,
                        LatestMessage = latestMessage != null ? latestMessage.MessageContent : "[No Message]",
                        SendDatetime = latestMessage != null ? latestMessage.SendDatetime : null,
                        Student = student != null ? _mapper.Map<StudentModel>(student) : null,
                    });
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

        [HttpGet("{chatGroupIdStr}/messages")]
        public async Task<IActionResult> GetMessagesChatGroup(string chatGroupIdStr, int messageIndex = 0, int limit = 5)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<IEnumerable<ChatMessageResponse>> response = new ApiResponse<IEnumerable<ChatMessageResponse>>();
            List<ChatMessageResponse> list = new List<ChatMessageResponse>();
            try
            {
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid Usser Id");
                }
                if (string.IsNullOrEmpty(chatGroupIdStr) || !int.TryParse(chatGroupIdStr, out int chatGroupId))
                {
                    throw new Exception("Invalid Chat Group Id");
                }
                var chatmessages = await _chatGroupService.GetMessagesInChatGroup(chatGroupId, messageIndex, limit);
                if (chatmessages == null)
                {
                    throw new Exception("Chat message not found");
                }
                foreach (var chatmessage in chatmessages)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    var user = await _userService.GetUserById(chatmessage.UserId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    if (user == null) continue;

                    list.Add(new ChatMessageResponse
                    {
                        ChatMessage = chatmessage,
                        Avatar = user.Avatar,
                        SenderName = user.UserId == userId ? "You" : user.FullName
                    });
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

        [HttpGet("{chatGroupIdStr}/members")]
        public async Task<IActionResult> GetMembersInChatGroup(string chatGroupIdStr)
        {
            ApiResponse<IEnumerable<MemberInChatGroupResponse>> response = new ApiResponse<IEnumerable<MemberInChatGroupResponse>>();
            List<MemberInChatGroupResponse> list = new List<MemberInChatGroupResponse>();
            try
            {
                if (string.IsNullOrEmpty(chatGroupIdStr) || !int.TryParse(chatGroupIdStr, out int chatGroupId))
                {
                    throw new Exception("Invalid Chat Group Id");
                }
                var chatGroup = await _chatGroupService.GetChatGroupWithUsersByChatGroupId(chatGroupId);
                if (chatGroup == null)
                {
                    throw new Exception("Chat group not found");
                }
                foreach (var user in chatGroup.Users)
                {
                    if (user == null) continue;

                    list.Add(new MemberInChatGroupResponse
                    {
                        UserId = user.UserId,
                        FullName = user.FullName,
                        Email = user.Email,
                        IsOwner = (user.UserId == chatGroup.OwnerId) ? true : false,
                    });
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

        [HttpPost("add-members")]
        public async Task<IActionResult> AddMembers([FromBody] AddMembersModel request)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            if (request == null || request.GroupId <= 0 || request.Emails == null || request.Emails.Count == 0)
            {
                return BadRequest("Invalid request.");
            }

            var result = await _chatGroupService.AddMembersToChatGroupAsync(request.GroupId, request.Emails);

            if (result)
            {
                response.Data = "Members added successfully.";
                return Ok(response);
            }
            else
            {
                response.Success = false;
                response.ErrorMessage = "Failed to add members.";
                return BadRequest(response);
            }
        }

        [HttpPost("personal/contact")]
        public async Task<IActionResult> SendMessageToGroupLeader([FromBody] CreatePersonalChatGroupModal request)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                if (string.IsNullOrEmpty(request.LeaderId) || !int.TryParse(request.LeaderId, out int leaderId) ||
                    string.IsNullOrEmpty(request.UserId) || !int.TryParse(request.UserId, out int userId) ||
                    string.IsNullOrEmpty(request.Message))
                {
                    throw new Exception("Invalid data.");
                }

                var result = await _chatGroupService.SendMessageToGroupLeader(leaderId, userId, request.Message);

                if (result)
                {
                    response.Data = "Send successfully, please wait for response from group leader.";
                    return Ok(response);
                }
                else
                {
                    throw new Exception("Failed to send message.");
                }
            }
            catch (Exception ex) 
            {
                    response.Success = false;
                    response.ErrorMessage = ex.Message;
                    return BadRequest(response);
            }
        }


        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveMemberOutOfGroupChat([FromBody] RemoveMemberOutOfChatGroupModel request)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            var result = await _chatGroupService.RemoveMemberOutOfGroupChat(request.UserId, request.ChatGroupId);

            if (result)
            {
                response.Data = "Member has been kicked out.";
                return Ok(response);
            }
            else
            {
                response.Success = false;
                response.ErrorMessage = "Failed to add members.";
                return BadRequest(response);
            }
        }
    }
}
