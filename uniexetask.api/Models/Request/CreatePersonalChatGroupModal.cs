namespace uniexetask.api.Models.Request
{
    public class CreatePersonalChatGroupModal
    {
        public required string ContactedUserId { get; set; }
        public required string ContactUserId { get; set; }
        public required string Message { get; set; }
    }
}
