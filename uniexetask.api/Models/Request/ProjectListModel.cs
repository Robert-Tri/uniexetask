using Microsoft.AspNetCore.Mvc;

namespace uniexetask.api.Models.Request
{
    public class ProjectListModel
    {
        public int ProjectId { get; set; }
        public string TopicCode { get; set; }
        public string TopicName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
    }
}
