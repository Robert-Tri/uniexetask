using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using uniexetask.shared.Models.Response;
using uniexetask.services.Interfaces;

namespace uniexetask.services.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatGroupService _chatGroupService;
        private readonly IUserService _userService;
        private static readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();

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
            if (newMessage == null) throw new HubException("Message cannot be saved");
            var chatgroup = await _chatGroupService.GetChatGroupWithUsersByChatGroupId(chatGroupId);
            if (chatgroup == null) throw new HubException();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var user = await _userService.GetUserById(newMessage.UserId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (user == null) throw new HubException("User not found");
            string messageContent = newMessage.MessageContent;
            if (newMessage.MessageContent.Length > 50)
            {
                messageContent = messageContent.Substring(0, 50) + "...";
            }
            ChatGroupResponse chatGroupResponse = new ChatGroupResponse
            {
                ChatGroup = chatgroup,
                LatestMessage = messageContent,
                SendDatetime = newMessage.SendDatetime
            };

            ChatMessageResponse messageResponse = new ChatMessageResponse
            {
                ChatMessage = newMessage,
                Avatar = user.Avatar,
                SenderName = user.FullName
            };

            List<int> userIds = new List<int>();
            foreach (var userr in chatgroup.Users)
            {
                userIds.Add(userr.UserId);
            }
            await Clients.Group(chatGroupIdStr).SendAsync("ReceiveMessages", messageResponse);
            await Clients.All.SendAsync("UpdateChatGroupWhenReceiveMessages", chatGroupResponse, userIds);
        }
    }
}
