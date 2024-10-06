using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IRolePermissionService
    {
        Task<IEnumerable<Permission>> GetPermissions();
        Task<IEnumerable<Feature>> GetFeatures();
        Task<IEnumerable<Permission>?> GetRolePermissionsByRole(string role);
        Task<IEnumerable<string>?> GetRoles();
        Task<bool> UpdateRolePermissionsAsync(string roleName, List<int> permissions);
    }
}
