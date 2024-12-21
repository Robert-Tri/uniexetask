using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IChatGroupRepository : IGenericRepository<ChatGroup>
    {
        Task<IEnumerable<ChatGroup>> GetAllChatGroups();
        Task<ChatGroup?> GetChatGroupByGroupId(int groupId);
        Task<ChatGroup?> GetChatGroupWithUsersByChatGroupIdAsync(int chatGroupId);
        Task<bool> IsUserInChatGroup(int chatGroupId, int userId);
    }
}
