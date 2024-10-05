namespace uniexetask.api.Models.Request
{
    public class GroupListModel
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; } = null!;

        public string SubjectName { get; set; }

        public bool HasMentor { get; set; }
    }
}
