using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
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

            if (chatGroup == null || !chatGroup.Type.Equals("Group"))
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

        public async Task<IEnumerable<ChatGroup>?> GetChatGroupByUserId(int userId)
        {
            var userWithChatGroups = await _unitOfWork.Users.GetUserWithChatGroupByUserIdAsyn(userId);
            if (userWithChatGroups == null || userWithChatGroups.ChatGroups == null) return null;
            List<ChatGroup> chatGroups = new List<ChatGroup>();
            foreach (var chatgroup in userWithChatGroups.ChatGroups)
            {
                if (chatgroup.Type.Equals("Personal"))
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
                chatGroups.Add(chatgroup);
            }
            return chatGroups;
        }

        public async Task<ChatGroup?> GetChatGroupWithUsersByChatGroupId(int chatGroupId)
        {
            return await _unitOfWork.ChatGroups.GetChatGroupWithUsersByChatGroupIdAsync(chatGroupId);
        }

        public async Task<ChatMessage?> GetLatestMessageInChatGroup(int chatGroupId)
        {
            return await _unitOfWork.ChatMessages.GetLatestMessageInChatGroup(chatGroupId);
        }

        public async Task<IEnumerable<ChatMessage?>> GetMessagesInChatGroup(int chatGroupId)
        {
            var chatMessages = await _unitOfWork.ChatMessages.GetMessagesInChatGroup(chatGroupId);
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

        public async Task<ChatMessage> SaveMessageAsync(int chatGroupId, int userId, string message)
        {
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
