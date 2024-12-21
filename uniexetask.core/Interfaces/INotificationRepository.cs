using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<Notification?> GetLatestNotification(int userId);
        Task<IEnumerable<Notification>?> GetNotificationsWithGroupInviteByUserId(int userId, int notificationIndex, int limit, string keyword);
        Task<int> GetNumberOfUnreadNotificationByUserId(int userId);
        Task<IEnumerable<Notification>?> GetUnreadNotificationsByUserId(int userId);
    }
}
