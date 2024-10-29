namespace uniexetask.api.Models.Request
{
    public class CreateTaskAssignModel
    {
        public int TaskId { get; set; }
        public List<int>? StudentsId { get; set; }
    }
}
