namespace uniexetask.shared.Models.Request
{
    public class SendGroupInviteModel
    {
        public required string SenderId { get; set; }
        public required string ReceiverId { get; set; }
        public required string GroupId { get; set; }
        public required string GroupName { get; set; }
    }
}
