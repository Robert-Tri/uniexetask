namespace uniexetask.api.Models.Request
{
    public class CreateTaskModel
    {
        public int ProjectId { get; set; }

        public string TaskName { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public List<int> AssignedMembers { get; set; }
        public List<TaskDetailsModel>? TaskDetailsModel { get; set; }

    }
}
