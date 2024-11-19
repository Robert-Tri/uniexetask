using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;

namespace uniexetask.infrastructure.Repositories
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        public GroupRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async Task<Group?> GetGroupWithProjectAsync(int groupId)
        {
            return await dbSet
                .Include(r => r.Projects)
                .FirstOrDefaultAsync(r => r.GroupId == groupId && r.IsCurrentPeriod);
        }

        public async Task<Group?> GetGroupWithSubjectAsync(int groupId)
        {
            return await dbSet
                .Include(r => r.Subject)
                .FirstOrDefaultAsync(r => r.GroupId == groupId && r.IsCurrentPeriod);
        }
        public async Task<IEnumerable<Group>> GetHasNoMentorGroupsWithGroupMembersAndStudent()
        {
            return await dbSet.Include(g => g.GroupMembers).ThenInclude(g => g.Student).Where(g => g.HasMentor == false && g.IsCurrentPeriod).AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<Group>> GetApprovedGroupsWithGroupMembersAndStudent()
        {
            return await dbSet.Include(g => g.GroupMembers).ThenInclude(g => g.Student).Where(g => g.Status == nameof(GroupStatus.Approved) && g.IsCurrentPeriod).AsNoTracking().ToListAsync();
        }
    }
}
