namespace uniexetask.api.Models.Request
{
    public class RejectRegTopicModel
    {
        public required int RegTopicId { get; set; }
        public string? RejectionReason { get; set; }
    }
}
