using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;

namespace uniexetask.infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async Task<Notification?> GetLatestNotification(int userId)
        {
            return await dbSet
                .Where(n => n.ReceiverId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Notification>?> GetNotificationsWithGroupInviteByUserId(int userId, int notificationIndex, int limit, string keyword)
        {
            return await dbSet
                .Include(n => n.GroupInvites)
                .Where(c => c.ReceiverId == userId && c.Message.Contains(keyword))
                .OrderByDescending(m => m.CreatedAt)
                .Skip(notificationIndex * limit)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetNumberOfUnreadNotificationByUserId(int userId)
        {
            return await dbSet
                .Where(n => n.ReceiverId == userId && n.Status != nameof(NotificationStatus.Read))
                .CountAsync();
        }

        public async Task<IEnumerable<Notification>?> GetUnreadNotificationsByUserId(int userId)
        {
            return await dbSet
                .Where(n => n.ReceiverId == userId && n.Status != nameof(NotificationStatus.Read))
                .ToListAsync();
        }
    }
}
