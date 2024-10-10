using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<ChatGroup>?> GetChatGroupByUserId(int userId)
        {
            var chatMessage = await _unitOfWork.ChatMessages.GetFirstMessagesInChatGroupByUserIdAsync(userId);
            if (chatMessage == null) return null;
            List<ChatGroup> chatGroups = new List<ChatGroup>();
            foreach (var message in chatMessage)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var chatGroup = await _unitOfWork.ChatGroups.GetByIDAsync(message.ChatGroupId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                if (chatGroup == null) continue;
                if (chatGroup.Type.Equals("Personal"))
                {
                    var messageByOther = await _unitOfWork.ChatMessages.GetFirstMessageInChatGroupByUserIdAsync(userId, chatGroup.ChatGroupId);
                    if (messageByOther == null) continue;
                    var user = await _unitOfWork.Users.GetByIDAsync(messageByOther.UserId);
                    if (user == null) continue;
                    chatGroup.ChatGroupName = user.FullName;
                    chatGroup.ChatGroupAvatar = user.Avatar;
                }
                chatGroups.Add(chatGroup);
            }
            return chatGroups;
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
