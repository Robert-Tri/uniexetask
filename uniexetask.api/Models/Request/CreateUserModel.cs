using uniexetask.core.Models;

namespace uniexetask.api.Models.Request
{
    public class CreateUserModel
    {
      
        public string FullName { get; set; } = null!;

        //public string? Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }
        //public string? Avatar { get; set; }

        public int CampusId { get; set; }

        //public bool Status { get; set; }

        public int RoleId { get; set; }
    }
}
