using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> CreateGroupInvite(int senderId, int receiverId, int groupId, string groupName);
        Task<IEnumerable<Notification>?> GetNotificationsWithGroupInviteByUserId(int userId, int notificationIndex, int limit, string keyword);
        Task<int> GetNumberOfUnreadNotificationByUserId(int userId);
        Task<GroupInvite?> HandleGroupInviteResponse(string choice, int notificationId, int groupId, int inviteeId);
        Task<bool> MarkNotificationsAsRead(int userId);
    }
}
