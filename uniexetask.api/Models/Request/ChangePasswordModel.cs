using uniexetask.core.Models;

namespace uniexetask.api.Models.Request
{
    public class ChangePasswordModel
    {


        public string? oldPassword { get; set; } = null!;

        public string? newPassword { get; set; } = null!;

    }
}
