namespace uniexetask.shared.Models.Request
{
    public class NotificationModel
    {
        public int NotificationId { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public string Message { get; set; } = null!;

        public string Type { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = null!;

        public virtual ICollection<GroupInviteModel> GroupInvites { get; set; } = new List<GroupInviteModel>();
    }
}
