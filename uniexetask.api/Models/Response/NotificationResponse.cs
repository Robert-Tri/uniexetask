using uniexetask.api.Models.Request;

namespace uniexetask.api.Models.Response
{
    public class NotificationResponse
    {
        public IEnumerable<NotificationModel> Notifications { get; set; }
        public int UnreadNotifications { get; set; }
        public NotificationResponse() 
        {
            Notifications = new List<NotificationModel>();
            UnreadNotifications = 0;
        }
    }
}
