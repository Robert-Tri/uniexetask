namespace uniexetask.api.Models.Request
{
    public class TaskModel
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public decimal ProgressPercentage { get; set; }
        public Boolean IsDeleted { get; set; }
        public IEnumerable<TaskAssignModel> TaskAssigns { get; set; }
    }
}
