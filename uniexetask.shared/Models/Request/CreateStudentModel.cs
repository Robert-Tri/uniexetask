namespace uniexetask.shared.Models.Request
{
    public class CreateStudentModel
    {
        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public int CampusId { get; set; }

        public int LectureId { get; set; }

        public int SubjectId { get; set; }

        public string StudentCode { get; set; } = null!;

        public string Major { get; set; } = null!;
    }
}
