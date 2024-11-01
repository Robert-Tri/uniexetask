namespace uniexetask.api.Models.Request
{
    public class TaskAssignModel
    {
        public int TaskAssignId { get; set; }
        public int TaskId { get; set; }
        public int StudentId { get; set; }
        public DateTime AssignedDate { get; set; }
    }
}
