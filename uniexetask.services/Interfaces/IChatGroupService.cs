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
        Task<ChatGroup?> GetChatGroupByChatGroupId(int chatGroupId);
        Task<IEnumerable<ChatGroup>?> GetChatGroupByUserId(int userId, int chatGroupIndex, int limit, string keyword);
        Task<ChatGroup?> GetChatGroupWithUsersByChatGroupId(int chatGroupId);
        Task<ChatMessage?> GetLatestMessageInChatGroup(int chatGroupId);
        Task<IEnumerable<ChatMessage>?> GetMessagesInChatGroup(int chatGroupId, int messageIndex, int limit);
        System.Threading.Tasks.Task MarkAsReadAsync(string chatGroupId, string userId, int lastReadMessageId);
        Task<bool> RemoveMemberOutOfGroupChat(int userId, int chatGroupId);
        Task<ChatMessage?> SaveMessageAsync(int chatGroupId, int userId, string message);
    }
}
