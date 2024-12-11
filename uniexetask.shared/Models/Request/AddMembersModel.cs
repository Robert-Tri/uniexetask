namespace uniexetask.shared.Models.Request
{
    public class AddMembersModel
    {
        public required int GroupId { get; set; }
        public required List<string> Emails { get; set; }
    }
}
