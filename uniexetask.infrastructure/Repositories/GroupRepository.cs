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
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        public GroupRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        /*        public async Task<Group?> GetGroupWithProjectAsync(int groupId)
                {
                    return await dbSet
                        .Include(r => r.Project)
                        .FirstOrDefaultAsync(r => r.GroupId == groupId);
                }*/

        public async Task<IEnumerable<Group>> GetAllGroups()
        {
            return await dbSet.ToListAsync();
        }

    }
}
