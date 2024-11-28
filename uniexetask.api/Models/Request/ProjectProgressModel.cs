namespace uniexetask.api.Models.Request
{
    public class ProjectProgressModel
    {
        public int ProjectId { get; set; }

        public decimal ProgressPercentage { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string? Note { get; set; }

        public bool IsDeleted { get; set; }

    }
}
