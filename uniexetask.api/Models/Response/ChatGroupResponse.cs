using uniexetask.core.Models;

namespace uniexetask.api.Models.Response
{
    public class ChatGroupResponse
    {
        public required ChatGroup ChatGroup { get; set; }

        public string LatestMessage { get; set; } = null!;
    }
}
