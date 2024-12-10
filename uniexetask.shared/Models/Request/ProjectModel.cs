namespace uniexetask.shared.Models.Request
{
    public class ProjectModel
    {

        public int GroupId { get; set; }

        public int TopicId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int SubjectId { get; set; }

        public bool IsCurrentPeriod { get; set; }

        public string Status { get; set; } = null!;

        public bool IsDeleted { get; set; }

    }
}
