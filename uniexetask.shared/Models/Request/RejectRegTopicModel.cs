namespace uniexetask.shared.Models.Request
{
    public class RejectRegTopicModel
    {
        public required int RegTopicId { get; set; }
        public string? RejectionReason { get; set; }
    }
}
