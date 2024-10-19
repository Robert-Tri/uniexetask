using uniexetask.core.Models;

namespace uniexetask.api.Models.Response
{
    public class MemberInChatGroupResponse
    {
        public required int UserId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required bool IsOwner { get; set; }
    }
}
