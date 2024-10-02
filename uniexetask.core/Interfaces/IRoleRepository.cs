using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task<Role?> GetRoleWithPermissionsAsync(int roleId);
    }
}
