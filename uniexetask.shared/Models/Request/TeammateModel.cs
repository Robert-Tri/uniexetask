namespace uniexetask.shared.Models.Request
{
    public class TeammateModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Major { get; set; }
        public int? StudentId { get; set; }
        public string StudentCode { get; set; }
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; }
        public string Role { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public string MentorName { get; set; }
        public string Status { get; set; }
    }
}
