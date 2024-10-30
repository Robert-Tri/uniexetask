using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class ChatGroupService : IChatGroupService
    {
        public IUnitOfWork _unitOfWork;
        public ChatGroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddMembersToChatGroupAsync(int chatGroupId, List<string> emails)
        {
            var chatGroup = await _unitOfWork.ChatGroups.GetByIDAsync(chatGroupId);

            if (chatGroup == null || chatGroup.Type != nameof(ChatGroupType.Group))
                return false;
            foreach (var email in emails) 
            {
                var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
                if (user == null) continue;
                chatGroup.Users.Add(user);
            }
            _unitOfWork.Save();
            return true;
        }

        public async Task<IEnumerable<ChatGroup>?> GetChatGroupByUserId(int userId, int chatGroupIndex, int limit, string keyword)
        {
            var userWithChatGroups = await _unitOfWork.Users.GetUserWithChatGroupByUserIdAsyn(userId);
            if (userWithChatGroups == null || userWithChatGroups.ChatGroups == null) return null;
            var chatGroups = userWithChatGroups.ChatGroups;
            foreach (var chatgroup in chatGroups)
            {
                if (chatgroup.Type == nameof(ChatGroupType.Personal))
                {
                    var chatGroupWithUsers = await _unitOfWork.ChatGroups.GetChatGroupWithUsersByChatGroupIdAsync(chatgroup.ChatGroupId);
                    if (chatGroupWithUsers == null || chatGroupWithUsers.Users == null) continue;
                    foreach (var user in chatGroupWithUsers.Users)
                    {
                        if (user.UserId == userId) continue;
                        chatgroup.ChatGroupName = user.FullName;
                        chatgroup.ChatGroupAvatar = user.Avatar;
                        break;
                    }
                }
            }
            var chatGroupsAfterPaging = chatGroups
                .Where(c => c.ChatGroupName.Contains(keyword))
                .Skip(chatGroupIndex * limit)
                .Take(limit)
                .OrderByDescending(e => e.LatestActivity)
                .ToList();
            return chatGroupsAfterPaging;
        }

        public async Task<ChatGroup?> GetChatGroupWithUsersByChatGroupId(int chatGroupId)
        {
            return await _unitOfWork.ChatGroups.GetChatGroupWithUsersByChatGroupIdAsync(chatGroupId);
        }

        public async Task<ChatMessage?> GetLatestMessageInChatGroup(int chatGroupId)
        {
            return await _unitOfWork.ChatMessages.GetLatestMessageInChatGroup(chatGroupId);
        }

        public async Task<IEnumerable<ChatMessage?>> GetMessagesInChatGroup(int chatGroupId, int messageIndex, int limit)
        {
            var chatMessages = await _unitOfWork.ChatMessages.GetMessagesInChatGroup(chatGroupId, messageIndex, limit);
            if (chatMessages == null) return Enumerable.Empty<ChatMessage?>();
            return chatMessages;
        }

        public System.Threading.Tasks.Task MarkAsReadAsync(string chatGroupId, string userId, int lastReadMessageId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveMemberOutOfGroupChat(int userId, int chatGroupId)
        {
            var chatGroup = await _unitOfWork.ChatGroups.GetChatGroupWithUsersByChatGroupIdAsync(chatGroupId);
            if (chatGroup == null) return false;
            var userToRemove = chatGroup.Users.FirstOrDefault(u => u.UserId == userId);
            if (userToRemove != null)
            {
                chatGroup.Users.Remove(userToRemove);
                _unitOfWork.Save();
                return true;
            }
            return false;
        }

        public async Task<ChatMessage?> SaveMessageAsync(int chatGroupId, int userId, string message)
        {
            var chatgroup = await _unitOfWork.ChatGroups.GetByIDAsync(chatGroupId);
            if (chatgroup == null) return null;
            chatgroup.LatestActivity = DateTime.Now;
            _unitOfWork.ChatGroups.Update(chatgroup);
            var chatMessage = new ChatMessage
            {
                ChatGroupId = chatGroupId,
                UserId = userId,
                MessageContent = message
            };
            await _unitOfWork.ChatMessages.InsertAsync(chatMessage);
            _unitOfWork.Save();
            return chatMessage;
        }
    }
}
