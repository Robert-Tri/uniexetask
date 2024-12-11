namespace uniexetask.shared.Models.Request
{
    public class TopicListModel
    {
        public int TopicId { get; set; }

        public string TopicCode { get; set; } = null!;

        public string TopicName { get; set; } = null!;

        public string Description { get; set; } = null!;
    }
}
