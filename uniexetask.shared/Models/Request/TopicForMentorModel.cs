namespace uniexetask.shared.Models.Request
{
    public class TopicForMentorModel
    {

        public int MentorId { get; set; }

        public string TopicCode { get; set; } = null!;

        public string TopicName { get; set; } = null!;

        public string Description { get; set; } = null!;

        public bool IsRegistered { get; set; }
    }
}
