namespace uniexetask.api.Models.Request
{
    public class CreateGroupWithMemberModel
    {
        public GroupModel Group { get; set; } = null!;

        public List<string> StudentCodes { get; set; } = new List<string>(); 
    }

}
