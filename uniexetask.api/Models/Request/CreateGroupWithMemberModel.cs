namespace uniexetask.api.Models.Request
{
    public class CreateGroupWithMemberModel
    {
        public GroupModel Group { get; set; } = null!;
        public GroupMemberModel Member { get; set; } = null!;
    }

}
