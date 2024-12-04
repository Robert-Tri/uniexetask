namespace uniexetask.api.Models.Request
{
    public class CreateMentorModel
    {
        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public int CampusId { get; set; }

        public string Specialty { get; set; } = null!;
    }
}
