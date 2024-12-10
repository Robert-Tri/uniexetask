using uniexetask.core.Models;

namespace uniexetask.shared.Models.Response
{
    public class ChatMessageResponse
    {
        public required ChatMessage ChatMessage { get; set; }
        public string SenderName { get; set; } = null!;
        public string? Avatar { get; set; }
    }
}
