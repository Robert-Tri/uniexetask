using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;

namespace uniexetask.infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await dbSet.FirstOrDefaultAsync(u =>
                u.Name.Equals(roleName));
        }

        public async Task<Role?> GetRoleWithPermissionsAsync(int roleId)
        {
            return await dbSet
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }
    }
}
