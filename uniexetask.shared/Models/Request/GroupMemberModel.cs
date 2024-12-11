namespace uniexetask.shared.Models.Request
{
    public class GroupMemberModel
    {
        public int GroupId { get; set; }

        public int StudentId { get; set; }

        public string Role { get; set; } = null!;
    }
}
