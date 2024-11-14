namespace uniexetask.api.Models.Request
{
    public class TaskDetailsModel
    {
        public int? TaskDetailId { get; set; }
        public int? TaskId { get; set; }

        public string TaskDetailName { get; set; } = null!;

        public decimal ProgressPercentage { get; set; }

        public bool? IsCompleted { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
