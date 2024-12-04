namespace uniexetask.api.Models.Request
{
    public class ProjectProgressEachMember
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Avatar { get; set; }
        public int NumberOfTaskNotStarted { get; set; }
        public int NumberOfTaskInProgress { get; set; }
        public int NumberOfTaskCompleted { get; set; }
        public int NumberOfTaskOverdue { get; set; }
        public string? Role { get; set; } = null!;

    }
}
