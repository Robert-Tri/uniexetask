using uniexetask.core.Models;
using uniexetask.shared.Models.Request;

namespace uniexetask.shared.Models.Response
{
    public class ChatGroupResponse
    {
        public required ChatGroup ChatGroup { get; set; }
        public required string LatestMessage { get; set; }
        public DateTime? SendDatetime { get; set; }
        // Properties for personal chat
        public StudentModel? Student { get; set; }
        public int? UserId { get; set; }

    }
}
