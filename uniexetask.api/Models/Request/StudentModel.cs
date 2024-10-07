namespace uniexetask.api.Models.Request
{
    public class StudentModel
    {
        public int StudentId { get; set; }

        public int UserId { get; set; }

        public int LecturerId { get; set; }

        public string StudentCode { get; set; } = null!;

        public string Major { get; set; } = null!;

        public bool IsEligible { get; set; }
    }
}
