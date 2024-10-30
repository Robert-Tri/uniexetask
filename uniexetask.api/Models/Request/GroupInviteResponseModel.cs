namespace uniexetask.api.Models.Request
{
    public class GroupInviteResponseModel
    {
        public required string Choice { get; set; }
        public required int NotificationId { get; set; }
        public required int GroupId { get; set; }
        public required int InviteeId { get; set; }
    }
}
