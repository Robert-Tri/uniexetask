namespace uniexetask.shared.Models.Request
{
    public class RemoveMemberOutOfChatGroupModel
    {
        public required int UserId { get; set; }
        public required int ChatGroupId { get; set; }
    }
}
