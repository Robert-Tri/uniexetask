namespace uniexetask.shared.Models.Request
{
    public class GroupInviteModel
    {
        public int GroupId { get; set; }

        public int NotificationId { get; set; }

        public int InviterId { get; set; }

        public int InviteeId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string Status { get; set; } = null!;
    }
}
