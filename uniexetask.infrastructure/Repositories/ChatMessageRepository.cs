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
    public class ChatMessageRepository : GenericRepository<ChatMessage>, IChatMessageRepository
    {
        public ChatMessageRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async Task<ChatMessage?> GetFirstMessageInChatGroupByUserIdAsync(int userId, int chatGroupId)
        {
            return await dbSet.Where(c => c.UserId != userId && c.ChatGroupId == chatGroupId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ChatMessage?>> GetFirstMessagesInChatGroupByUserIdAsync(int userId)
        {
            return await dbSet
                .Where(m => m.UserId == userId)
                .GroupBy(m => m.ChatGroupId)
                .Select(g => g.OrderBy(m => m.SendDatetime)
                .FirstOrDefault())
                .ToListAsync();
        }

        public async Task<ChatMessage?> GetLatestMessageInChatGroup(int chatGroupId)
        {
            return await dbSet
                    .Where(c => c.ChatGroupId == chatGroupId)
                    .OrderByDescending(m => m.SendDatetime)
                    .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ChatMessage?>> GetMessagesInChatGroup(int chatGroupId)
        {
            return await dbSet.Where(c => c.ChatGroupId == chatGroupId)
                .OrderBy(m => m.SendDatetime)
                .ToListAsync();
        }
    }
}
