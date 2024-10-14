using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
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

        public ChatGroupController(IChatGroupService chatGroupService, IUserService userService)
        {
            _chatGroupService = chatGroupService;
            _userService = userService;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetChatGroupByUser()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<IEnumerable<ChatGroupResponse>> response = new ApiResponse<IEnumerable<ChatGroupResponse>>();
            List<ChatGroupResponse> list = new List<ChatGroupResponse>();
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                response.Success = false;
                response.ErrorMessage = "User Id not found";
                return NotFound(response);
            }
            var chatgroups = await _chatGroupService.GetChatGroupByUserId(userId);
            if (chatgroups == null) 
            {
                response.Success = false;
                response.ErrorMessage = "Chat group not found";
                return NotFound(response);
            }
            int recceiverId = 0;
            foreach (var chatgroup in chatgroups) 
            {
                if (chatgroup.Type.Equals("Personal") && recceiverId == 0)
                {
                    var messages = await _chatGroupService.GetMessagesInChatGroup(chatgroup.ChatGroupId);
                    foreach (var message in messages)
                    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        if (message.UserId != userId) 
                        {
                            recceiverId = message.UserId;
                            break;
                        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }
                }
                var latestMessage = await _chatGroupService.GetLatestMessageInChatGroup(chatgroup.ChatGroupId);
                if (latestMessage == null) continue;
                list.Add(new ChatGroupResponse
                {
                    ChatGroup = chatgroup,
                    LatestMessage = latestMessage.MessageContent,
                    ReceiverId = recceiverId
                });
            }
            response.Data = list;
            return Ok(response);
        }

        [HttpGet("{chatGroupIdStr}/messages")]
        public async Task<IActionResult> GetMessagesChatGroup(string chatGroupIdStr)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<IEnumerable<ChatMessageResponse>> response = new ApiResponse<IEnumerable<ChatMessageResponse>>();
            List<ChatMessageResponse> list = new List<ChatMessageResponse>();
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId) || string.IsNullOrEmpty(chatGroupIdStr) || !int.TryParse(chatGroupIdStr, out int chatGroupId))
            {
                response.Success = false;
                response.ErrorMessage = "User Id not found";
                return NotFound(response);
            }
            var chatmessages = await _chatGroupService.GetMessagesInChatGroup(chatGroupId);
            if (chatmessages == null)
            {
                response.Success = false;
                response.ErrorMessage = "Chat message not found";
                return NotFound(response);
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
    }
}
