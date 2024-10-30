using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IGroupInviteRepository : IGenericRepository<GroupInvite>
    {
        Task<GroupInvite?> GetGroupInviteByNotificationId(int notificationId);
        Task<GroupInvite?> GetPendingInvite(int receiverId, int groupId);
    }
}
