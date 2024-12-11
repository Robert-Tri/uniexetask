namespace uniexetask.shared.Models.Response
{
    public class SearchStudentCodeResponse
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string StudentCode { get; set; }
        public string? Avatar { get; set; }
    }
}
