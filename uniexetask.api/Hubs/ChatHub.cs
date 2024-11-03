using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
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

            ChatGroupResponse chatGroupResponse = new ChatGroupResponse
            {
                ChatGroup = chatgroup,
                LatestMessage = newMessage.MessageContent,
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
        public async Task SetUserOnline(string userId)
        {
            _userConnections[userId] = Context.ConnectionId;
            await Clients.All.SendAsync("UserOnline", userId);
        }

        public async Task SetUserOffline(string userId)
        {
            _userConnections.TryRemove(userId, out _);
            await Clients.All.SendAsync("UserOffline", userId);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await SetUserOnline(userId);

                // Send the current online users list to the newly connected user
                var onlineUsers = _userConnections.Keys.ToList();
                await Clients.Caller.SendAsync("ReceiveOnlineUsers", onlineUsers);

                // Notify all users that this user is now online
                await Clients.All.SendAsync("UserOnline", userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (userId != null)
            {
                await SetUserOffline(userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<List<string>> GetOnlineUsers()
        {
            return _userConnections.Keys.ToList();
        }
    }
}
