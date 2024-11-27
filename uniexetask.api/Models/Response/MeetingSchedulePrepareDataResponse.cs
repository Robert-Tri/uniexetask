namespace uniexetask.api.Models.Response
{
    public class MeetingSchedulePrepareDataResponse
    {
        public required int GroupId { get; set; }
        public required string GroupName { get; set; }
        public required string TopicCode { get; set; }
        public required string TopicName { get; set; }
        public required string TopicDescription { get; set; }
    }
}
