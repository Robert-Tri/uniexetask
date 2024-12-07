namespace uniexetask.api.Models.Request
{
    public class GroupMemberDetailsModel
    {
        public int? userId { get; set; }
        public string fullName { get; set; } = null!;

        public string email { get; set; }

        public string phone { get; set; }
        public string major { get; set; }

        public string studentCode { get; set; } = null!;
        public string mentorName { get; set; } = null!;
        public string role { get; set; }
    }
}
