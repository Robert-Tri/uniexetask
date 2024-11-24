using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class NotificationService : INotificationService
    {
        public IUnitOfWork _unitOfWork;
        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Notification> CreateGroupInvite(int senderId, int receiverId, int groupId, string groupName)
        {
            var senderExists = await _unitOfWork.Users.GetByIDAsync(senderId);
            var receiverExists = await _unitOfWork.Users.GetByIDAsync(receiverId);
            var groupExists = await _unitOfWork.Groups.GetByIDAsync(groupId);

            if (senderExists == null || receiverExists == null || groupExists == null)
            {
                throw new Exception("One or more users or the group do not exist.");
            }

            var existingInvite = await _unitOfWork.GroupInvites.GetPendingInvite(receiverId, groupId);
            if (existingInvite != null)
            {
                throw new Exception("An invite has already been sent to this user for this group.");
            }
            try
            {
                _unitOfWork.BeginTransaction();
                var notification = new Notification
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Message = $"You received an invitation to join the {groupName} group.",
                    Type = "Group_Request",
                    CreatedAt = DateTime.Now,
                    Status = "Sent"
                };
                await _unitOfWork.Notifications.InsertAsync(notification);
                _unitOfWork.Save();
                var groupInvite = new GroupInvite
                {
                    GroupId = groupId,
                    NotificationId = notification.NotificationId,
                    InviterId = senderId,
                    InviteeId = receiverId,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = "Pending"
                };
                await _unitOfWork.GroupInvites.InsertAsync(groupInvite);
                _unitOfWork.Save();
                _unitOfWork.Commit();
                return notification;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Notification>?> GetNotificationsWithGroupInviteByUserId(int userId, int notificationIndex, int limit, string keyword)
        {
            return await _unitOfWork.Notifications.GetNotificationsWithGroupInviteByUserId(userId, notificationIndex, limit, keyword);
        }

        public async Task<int> GetNumberOfUnreadNotificationByUserId(int userId)
        {
            return await _unitOfWork.Notifications.GetNumberOfUnreadNotificationByUserId(userId);
        }

        public async Task<GroupInvite?> HandleGroupInviteResponse(string choice, int notificationId, int groupId, int inviteeId)
        {
            var notificationExists = await _unitOfWork.Notifications.GetByIDAsync(notificationId);
            var inviteeExists = await _unitOfWork.Users.GetByIDAsync(inviteeId);
            var groupExists = await _unitOfWork.Groups.GetByIDAsync(groupId);

            if (notificationExists == null || inviteeExists == null || groupExists == null)
            {
                throw new Exception("The user, notification or the group do not exist.");
            }

            if (groupExists.IsDeleted) throw new Exception("The group has been deleted or is past the semester.");

            var studentExists = await _unitOfWork.Students.GetStudentByUserId(inviteeId);

            if (studentExists == null)  throw new Exception("The person invited to join the group is not a student.");

            if (notificationExists.Type != nameof(NotificationType.Group_Request)) throw new Exception("This notification is not an invitation to join a group.");

            var groupInvite = await _unitOfWork.GroupInvites.GetGroupInviteByNotificationId(notificationId);

            if (groupInvite == null) throw new Exception("Group invite not found.");

            if (choice.Equals("Accept", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    _unitOfWork.BeginTransaction();
                    groupInvite.Status = nameof(GroupInviteStatus.Accepted);
                    _unitOfWork.GroupInvites.Update(groupInvite);
                    _unitOfWork.Save();

                    var groupMember = new GroupMember
                    {
                        GroupId = groupExists.GroupId,
                        StudentId = studentExists.StudentId,
                        Role = nameof(GroupMemberRole.Member)
                    };
                    await _unitOfWork.GroupMembers.InsertAsync(groupMember);
                    _unitOfWork.Save();
                    var chatGroup = await _unitOfWork.ChatGroups.GetChatGroupByGroupId(groupExists.GroupId);
                    if (chatGroup == null) throw new Exception("Chat group for this group is not exist.");
                    chatGroup.Users.Add(inviteeExists);
                    await _unitOfWork.ChatGroups.InsertAsync(chatGroup);
                    _unitOfWork.Save();
                    _unitOfWork.Commit();
                    return groupInvite;
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            else if (choice.Equals("Reject", StringComparison.OrdinalIgnoreCase))
            {
                groupInvite.Status = nameof(GroupInviteStatus.Rejected);
                _unitOfWork.GroupInvites.Update(groupInvite);
                _unitOfWork.Save();
                return groupInvite;
            }
            return null;
        }

        public async Task<bool> MarkNotificationsAsRead(int userId)
        {
            var notifications = await _unitOfWork.Notifications.GetUnreadNotificationsByUserId(userId);

            if (notifications == null) return false;

            foreach (var notification in notifications)
            {
                notification.Status = nameof(NotificationStatus.Read);
                _unitOfWork.Notifications.Update(notification);
            }
            _unitOfWork.Save();
            return true;
        }
    }
}
