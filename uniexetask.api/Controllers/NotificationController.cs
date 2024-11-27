using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using uniexetask.api.Hubs;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(INotificationService notificationService, IUserService userService, IMapper mapper, IHubContext<NotificationHub> hubContext)
        {
            _notificationService = notificationService;
            _userService = userService;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationsByUser(int notificationIndex = 0, int limit = 10, string keyword = "")
        {
            ApiResponse<NotificationResponse> response = new ApiResponse<NotificationResponse>();
            try
            {
                var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                List<NotificationModel> list = new List<NotificationModel>();
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("User Id not found");
                }
                var notifications = await _notificationService.GetNotificationsWithGroupInviteByUserId(userId, notificationIndex, limit, keyword);
                var numberOfUnreadNotification = await _notificationService.GetNumberOfUnreadNotificationByUserId(userId);
                if (notifications == null)
                {
                    throw new Exception("Empty");
                }
                foreach (var notification in notifications)
                {
                    list.Add(_mapper.Map<NotificationModel>(notification));
                }
                response.Data = new NotificationResponse
                {
                    Notifications = list,
                    UnreadNotifications = numberOfUnreadNotification
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }

        [HttpPost("group-invite/send")]
        public async Task<IActionResult> SendGroupInvite([FromBody] SendGroupInviteModel request)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                if (string.IsNullOrEmpty(request.SenderId) || !int.TryParse(request.SenderId, out int senderId) 
                    && string.IsNullOrEmpty(request.ReceiverId) || !int.TryParse(request.ReceiverId, out int receiverId) 
                    && string.IsNullOrEmpty(request.GroupId) || !int.TryParse(request.GroupId, out int groupId)
                    && string.IsNullOrEmpty(request.GroupName))
                {
                    throw new Exception("Invalid Data");
                }

                var newNotification = await _notificationService.CreateGroupInvite(senderId, receiverId, groupId, request.GroupName);

                await _hubContext.Clients.User(request.ReceiverId).SendAsync("ReceiveNotification", newNotification);
                response.Data = "Group invitation sent successfully. Group invitations are valid for 24 hours from the time the invitation is created.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }

        }

        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkNotificationsAsRead()
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("User Id not found");
                }

                var result = await _notificationService.MarkNotificationsAsRead(userId);

                if (result)
                {
                    response.Data = "Notifications marked as read";
                    return Ok(response);
                }
                else
                {
                    throw new Exception("Failed to mark notifications as read");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }

        [HttpPost("group-invite/response")]
        public async Task<IActionResult> RespondToGroupInvite([FromBody] GroupInviteResponseModel request)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("User Id not found");
                }

                // Kiểm tra lựa chọn
                if (string.IsNullOrEmpty(request.Choice) || request.NotificationId <= 0 ||
                            request.GroupId <= 0 || request.InviteeId <= 0)
                {
                    throw new Exception("Invalid Data");
                }

                if (userId != request.InviteeId) throw new Exception("group invitation recipient does not match current user");

                var groupInvite = await _notificationService.HandleGroupInviteResponse(request.Choice, request.NotificationId, request.GroupId, request.InviteeId);

                if (groupInvite != null)
                {
                    var user = await _userService.GetUserById(request.InviteeId);
                    if (user != null) 
                    {
                        var newNotification = await _notificationService.CreateNotification(userId, groupInvite.InviterId, $"{user.FullName} has joined the group");
                        await _hubContext.Clients.User(groupInvite.InviterId.ToString()).SendAsync("ReceiveNotification", newNotification);

                    }
                    response.Data = $"You have {groupInvite.Status.ToLower()} to join the group!";
                    return Ok(response);
                }
                else
                {
                    throw new Exception("Failed to process the group invite response");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }
    }
}
