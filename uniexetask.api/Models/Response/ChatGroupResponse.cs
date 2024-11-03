using uniexetask.core.Models;

namespace uniexetask.api.Models.Response
{
    public class ChatGroupResponse
    {
        public required ChatGroup ChatGroup { get; set; }

        public required string LatestMessage { get; set; }
        public DateTime? SendDatetime { get; set; }

    }
}
