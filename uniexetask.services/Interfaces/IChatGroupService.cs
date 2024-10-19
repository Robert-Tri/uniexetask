using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IChatGroupService
    {
        Task<bool> AddMembersToChatGroupAsync(int groupId, List<string> emails);
        Task<IEnumerable<ChatGroup>?> GetChatGroupByUserId(int userId);
        Task<ChatMessage?> GetLatestMessageInChatGroup(int chatGroupId);
        Task<IEnumerable<ChatMessage?>> GetMessagesInChatGroup(int chatGroupId);
        System.Threading.Tasks.Task MarkAsReadAsync(string chatGroupId, string userId, int lastReadMessageId);
        Task<ChatMessage> SaveMessageAsync(int chatGroupId, int userId, string message);
    }
}
