namespace uniexetask.shared.Models.Request
{
    public class UserModel
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = null!;

        public string? Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }
        public string? Avatar { get; set; }

        public int CampusId { get; set; }

        public bool IsDeleted { get; set; }

        public int RoleId { get; set; }
    }
}
