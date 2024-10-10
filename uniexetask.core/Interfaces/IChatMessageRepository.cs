using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IChatMessageRepository : IGenericRepository<ChatMessage>
    {
        Task<IEnumerable<ChatMessage?>> GetFirstMessagesInChatGroupByUserIdAsync(int userId);
        Task<ChatMessage?> GetFirstMessageInChatGroupByUserIdAsync(int userId, int chatGroupId);
        Task<ChatMessage?> GetLatestMessageInChatGroup(int chatGroupId);
        Task<IEnumerable<ChatMessage?>> GetMessagesInChatGroup(int chatGroupId);
    }
}
