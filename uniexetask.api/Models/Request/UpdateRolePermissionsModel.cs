using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Models.Request
{
    public class UpdateRolePermissionsModel
    {
        public string RoleName { get; set; }
        public List<int> Permissions { get; set; }

        public UpdateRolePermissionsModel() 
        {
            RoleName = "";
            Permissions = new List<int>();
        }
    }
}
