using AutoMapper;
using AutoMapper.Execution;
using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Hubs;
using uniexetask.services.Interfaces;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;

namespace uniexetask.services
{
    public class NotificationService : INotificationService
    {
        public IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IConfigSystemService _configSystemService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IUnitOfWork unitOfWork, IUserService userService, IConfigSystemService configSystemService, IGroupMemberService groupMemberService, IMapper mapper, IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _configSystemService = configSystemService;
            _groupMemberService = groupMemberService;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        public async Task<bool> CreateGroupInvite(int senderId, int receiverId, int groupId, string groupName)
        {
            var senderExists = await _unitOfWork.Users.GetByIDAsync(senderId);
            var receiverExists = await _unitOfWork.Users.GetByIDAsync(receiverId);
            var groupExists = await _unitOfWork.Groups.GetByIDAsync(groupId);
            var studentSenderExists = await _unitOfWork.Students.GetStudentByUserId(senderId);
            var studentExists = await _unitOfWork.Students.GetStudentByUserId(receiverId);

            if (senderExists == null || receiverExists == null || groupExists == null)
            {
                throw new Exception("One or more users or the group do not exist.");
            }

            if (studentExists == null || studentSenderExists == null) throw new Exception("The inviter or invitee to the group is not a student.");

            var existingInvite = await _unitOfWork.GroupInvites.GetPendingInvite(receiverId, groupId);
            if (existingInvite != null)
            {
                throw new Exception("An invite has already been sent to this user for this group.");
            }

            var role = await _groupMemberService.GetRoleByUserId(senderId);
            if (role != "Leader")
            {
                throw new Exception("You are not a leader to perform this operation.");
            }

            if (groupExists.Status != nameof(GroupStatus.Initialized))
            {
                throw new Exception("Group is eligible, you cannot invite others to join the group.");
            }

            if (senderExists.CampusId != receiverExists.CampusId)
            {
                throw new Exception("You cannot add member from another campus.");
            }

            bool studentExistsInGroup = await _groupMemberService.CheckIfStudentInGroup(studentExists.StudentId);

            if (studentExists.SubjectId != studentSenderExists.SubjectId)
            {
                throw new Exception("You cannot add members from a different subject.");
            }
            var groupMembers = await _unitOfWork.GroupMembers.GetGroupMembersWithStudentAndUser(groupExists.GroupId);

            var maxMemberExe101 = (await _configSystemService.GetConfigSystems())
           .FirstOrDefault(config => config.ConfigName == "MAX_MEMBER_EXE101");
            var maxMemberExe201 = (await _configSystemService.GetConfigSystems())
            .FirstOrDefault(config => config.ConfigName == "MAX_MEMBER_EXE201");

            var subject = await _unitOfWork.Subjects.GetByIDAsync(studentExists.SubjectId);

            if (studentExists.SubjectId == 1 && groupMembers.Count() == maxMemberExe101.Number) throw new Exception($"The group already have enough {maxMemberExe101.Number} members for EXE101.");
            if (studentExists.SubjectId == 2 && groupMembers.Count() == maxMemberExe201.Number) throw new Exception($"The group already have enough {maxMemberExe201.Number} members for EXE201.");
            try
            {
                _unitOfWork.BeginTransaction();
                var notification = new Notification
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Message = $"You received an invitation to join the <b>{groupName}</b> group.",
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
                await _hubContext.Clients.User(receiverId.ToString()).SendAsync("ReceiveNotification", notification);
                return true;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw new Exception(ex.Message);
            }
        }

        public async Task<Notification> CreateNotification(int senderId, int receiverId, string message)
        {
            var senderExists = await _unitOfWork.Users.GetByIDAsync(senderId);
            var receiverExists = await _unitOfWork.Users.GetByIDAsync(receiverId);

            if (senderExists == null || receiverExists == null)
            {
                throw new Exception("One or more users do not exist.");
            }
            var notification = new Notification
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                Type = nameof(NotificationType.Info),
                CreatedAt = DateTime.Now,
                Status = "Sent"
            };
            await _unitOfWork.Notifications.InsertAsync(notification);
            _unitOfWork.Save();
            return notification;
        }

        public async Task<NotificationResponse> GetNotificationsWithGroupInviteByUserId(int userId, int notificationIndex, int limit, string keyword)
        {
            List<NotificationModel> list = new List<NotificationModel>();
            var notifications = await _unitOfWork.Notifications.GetNotificationsWithGroupInviteByUserId(userId, notificationIndex, limit, keyword);
            var numberOfUnreadNotification = await GetNumberOfUnreadNotificationByUserId(userId);
            if (notifications == null)
            {
                throw new Exception("Load data failed");
            }
            foreach (var notification in notifications)
            {
                list.Add(_mapper.Map<NotificationModel>(notification));
            }
            return new NotificationResponse 
            {
                Notifications = list,
                UnreadNotifications = numberOfUnreadNotification
            };
        }

        public async Task<int> GetNumberOfUnreadNotificationByUserId(int userId)
        {
            return await _unitOfWork.Notifications.GetNumberOfUnreadNotificationByUserId(userId);
        }

        public async Task<IEnumerable<GroupInvite>> GetPendingInvitesAsync()
        {
            return await _unitOfWork.GroupInvites.GetPendingInvites();
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
            var groupInvite = await _unitOfWork.GroupInvites.GetGroupInviteByNotificationId(notificationId);

            if (groupInvite == null) throw new Exception("Group invite not found.");

            if (choice.Equals("Reject", StringComparison.OrdinalIgnoreCase))
            {
                groupInvite.Status = nameof(GroupInviteStatus.Rejected);
                _unitOfWork.GroupInvites.Update(groupInvite);
                _unitOfWork.Save();
                return groupInvite;
            }

            if (groupExists.IsDeleted) throw new Exception("The group has been deleted or is past the semester.");

            var studentExists = await _unitOfWork.Students.GetStudentByUserId(inviteeId);

            if (studentExists == null)  throw new Exception("The person invited to join the group is not a student.");

            if (notificationExists.Type != nameof(NotificationType.Group_Request)) throw new Exception("This notification is not an invitation to join a group.");

            if (groupInvite.Status == nameof(GroupInviteStatus.Expired)) throw new Exception("Group invitation expired.");

            var groupMembers = await _unitOfWork.GroupMembers.GetGroupMembersWithStudentAndUser(groupExists.GroupId);

            var maxMemberExe101 = (await _configSystemService.GetConfigSystems())
           .FirstOrDefault(config => config.ConfigName == "MAX_MEMBER_EXE101");
            var maxMemberExe201 = (await _configSystemService.GetConfigSystems())
            .FirstOrDefault(config => config.ConfigName == "MAX_MEMBER_EXE201");

            var subject = await _unitOfWork.Subjects.GetByIDAsync(studentExists.SubjectId);

            if (studentExists.SubjectId == 1 && groupMembers.Count() == maxMemberExe101.Number) throw new Exception($"The group already have enough {maxMemberExe101.Number} members for EXE101.");
            if (studentExists.SubjectId == 2 && groupMembers.Count() == maxMemberExe201.Number) throw new Exception($"The group already have enough {maxMemberExe201.Number} members for EXE201.");

            if (choice.Equals("Accept", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    _unitOfWork.BeginTransaction();
                    groupInvite.Status = nameof(GroupInviteStatus.Accepted);
                    groupInvite.UpdatedDate = DateTime.Now;
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
                    _unitOfWork.ChatGroups.Update(chatGroup);
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

        public void UpdateGroupInviteAsync(GroupInvite invite)
        {
            _unitOfWork.GroupInvites.Update(invite);
            _unitOfWork.Save();
        }
    }
}
