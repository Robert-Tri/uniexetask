namespace uniexetask.api.Models.Request
{
    public class CreatePersonalChatGroupModal
    {
        public required string LeaderId { get; set; }
        public required string UserId { get; set; }
        public required string Message { get; set; }
    }
}
