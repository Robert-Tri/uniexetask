using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;

namespace uniexetask.infrastructure.Repositories
{
    public class ChatGroupRepository : GenericRepository<ChatGroup>, IChatGroupRepository
    {
        public ChatGroupRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async Task<ChatGroup?> GetChatGroupByGroupId(int groupId)
        {
            return await dbSet
                .Include(c => c.Users)
                .FirstOrDefaultAsync(u => u.GroupId == groupId && !u.IsDeleted);
        }

        public async Task<ChatGroup?> GetChatGroupWithUsersByChatGroupIdAsync(int chatGroupId)
        {
            return await dbSet
                .Include(x => x.Users)
                .FirstOrDefaultAsync(u => u.ChatGroupId == chatGroupId && !u.IsDeleted);
        }

        public async Task<bool> IsUserInChatGroup(int chatGroupId, int userId)
        {
            return await dbSet
                .Where(cg => cg.ChatGroupId == chatGroupId && !cg.IsDeleted)
                .SelectMany(cg => cg.Users)
                .AnyAsync(u => u.UserId == userId); // Kiểm tra UserId
        }
    }
}
