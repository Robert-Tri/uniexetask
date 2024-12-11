namespace uniexetask.shared.Models.Response
{
    public class MemberInChatGroupResponse
    {
        public required int UserId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required bool IsOwner { get; set; }
        public required bool IsMentor { get; set; }
    }
}
