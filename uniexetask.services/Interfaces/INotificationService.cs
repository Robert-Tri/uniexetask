using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;
using uniexetask.shared.Models.Response;

namespace uniexetask.services.Interfaces
{
    public interface INotificationService
    {
        Task<bool> CreateGroupInvite(int senderId, int receiverId, int groupId, string groupName);
        Task<Notification> CreateNotification(int senderId, int receiverId, string message);
        Task<NotificationResponse> GetNotificationsWithGroupInviteByUserId(int userId, int notificationIndex, int limit, string keyword);
        Task<int> GetNumberOfUnreadNotificationByUserId(int userId);
        Task<IEnumerable<GroupInvite>> GetPendingInvitesAsync();
        Task<GroupInvite?> HandleGroupInviteResponse(string choice, int notificationId, int groupId, int inviteeId);
        Task<bool> MarkNotificationsAsRead(int userId);
        void UpdateGroupInviteAsync(GroupInvite invite);
    }
}
