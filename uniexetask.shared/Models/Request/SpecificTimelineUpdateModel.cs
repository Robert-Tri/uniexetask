namespace uniexetask.shared.Models.Request
{
    public class SpecificTimelineUpdateModel
    {
        public int TimelineId {  get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int SubjectId { get; set; }
    }
}
