namespace uniexetask.api.Models.Request
{
    public class UpdateRegTopicModel
    {
        public int RegTopicId { get; set; }

        public string TopicName { get; set; } = null!;

        public string Description { get; set; } = null!;

    }
}
