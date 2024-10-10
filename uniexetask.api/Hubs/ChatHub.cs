using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.Json;
using uniexetask.api.Models.Response;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatGroupService _chatGroupService;
        private readonly IUserService _userService;


        public ChatHub(IChatGroupService chatGroupService, IUserService userService)
        {
            _chatGroupService = chatGroupService;
            _userService = userService;
        }

        public async Task JoinChatGroup(string chatGroupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatGroupId);
        }

        public async Task LeaveChatGroup(string chatGroupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatGroupId);
        }

        public async Task SendMessage(string userIdStr, string chatGroupIdStr, string message)
        {
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId) || string.IsNullOrEmpty(chatGroupIdStr) || !int.TryParse(chatGroupIdStr, out int chatGroupId))
            {
                throw new HubException("User not authenticated");
            }

            var newMessage = await _chatGroupService.SaveMessageAsync(chatGroupId, userId, message);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var user = await _userService.GetUserById(newMessage.UserId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (user == null) throw new HubException("User not found");

            ChatMessageResponse response = new ChatMessageResponse
            {
                ChatMessage = newMessage,
                Avatar = user.Avatar,
                SenderName = user.FullName
            };
            await Clients.Group(chatGroupIdStr).SendAsync("ReceiveMessages", response);
        }

        public async Task MarkAsRead(string chatGroupId, int lastReadMessageId)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("User not authenticated");
            }

            await _chatGroupService.MarkAsReadAsync(chatGroupId, userId, lastReadMessageId);

            await Clients.Group(chatGroupId).SendAsync("MessageRead", new { UserId = userId, LastReadMessageId = lastReadMessageId });
        }
    }
}
