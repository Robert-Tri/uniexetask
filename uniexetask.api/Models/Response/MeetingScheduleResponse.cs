namespace uniexetask.api.Models.Response
{
    public class MeetingScheduleResponse
    {
        public required int ScheduleId { get; set; }
        public required string MeetingScheduleName { get; set; }
        public required string GroupName { get; set; }
        public required string TopicName { get; set; }
        public required string MentorName { get; set; }
        public required string Location { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required string Type { get; set; }
        public required string Content { get; set; }
        public string? Url { get; set; }
        public bool IsDeleted { get; set; }
    }
}
