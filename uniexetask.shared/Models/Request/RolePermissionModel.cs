using uniexetask.core.Models;

namespace uniexetask.shared.Models.Request
{
    public class RolePermissionModel
    {
        public required IEnumerable<Permission> Permissions { get; set; }
        public required IEnumerable<Feature> Features { get; set; }
        public required IEnumerable<Permission> PermissionsWithRole {  get; set; }
    }
}
