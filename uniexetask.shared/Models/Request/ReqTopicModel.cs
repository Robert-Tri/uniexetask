namespace uniexetask.shared.Models.Request
{
    public class ReqTopicModel
    {
        public int GroupId { get; set; }

        public string TopicCode { get; set; } = null!;

        public string TopicName { get; set; } = null!;

        public string Description { get; set; } = null!;
        public string? RejectionReason { get; set; }

        public bool Status { get; set; }
    }
}
