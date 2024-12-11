namespace uniexetask.shared.Models.Request
{
    public class RegMemberFormModel
    {

        public int GroupId { get; set; }

        public string Description { get; set; } = null!;

        public bool Status { get; set; }
    }
}
