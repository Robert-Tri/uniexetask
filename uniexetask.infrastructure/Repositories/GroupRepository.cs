using Microsoft.EntityFrameworkCore;
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
            return await dbSet.Include(g => g.GroupMembers).ThenInclude(g => g.Student).ThenInclude(g => g.User).Where(g => g.HasMentor == false && g.IsDeleted == false && g.IsCurrentPeriod).AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<Group>> GetApprovedGroupsWithGroupMembersAndStudent()
        {
            return await dbSet.Include(g => g.GroupMembers).ThenInclude(g => g.Student).Where(g => g.Status == nameof(GroupStatus.Approved) && g.IsCurrentPeriod).AsNoTracking().ToListAsync();
        }

        public async Task<Mentor?> GetMentorInGroup(int groupId)
        {
            return await dbSet.Where(g => g.GroupId == groupId)
                                  .Select(g => g.Mentors.FirstOrDefault())
                                  .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Group>> SearchGroupsByGroupNameAsync(int mentorId, string query)
        {
            return await dbSet
                        .Where(u => EF.Functions.Like(u.GroupName, $"%{query}%") && u.Mentors.Any(m => m.MentorId == mentorId) && !u.IsDeleted && u.IsCurrentPeriod)
                        .Take(5)
                        .ToListAsync();
        }
        
        public async Task<IEnumerable<Group>> GetCurrentPeriodGroupsWithMembersAndMentor()
        {
            return await dbSet.Include(g => g.GroupMembers).ThenInclude(gm => gm.Student).ThenInclude(s => s.User).Include(g => g.Mentors).ThenInclude(m => m.User).Where(g => g.IsCurrentPeriod && !g.IsDeleted).AsNoTracking().ToListAsync();
        }

        public async System.Threading.Tasks.Task RemoveMentorFromGroup(int groupId)
        {
            var group = await dbSet.Include(g => g.Mentors).FirstOrDefaultAsync(g => g.GroupId == groupId);
            if(group == null)
                throw new Exception("Group not found");
            group.Mentors.Remove(group.Mentors.First());
        }
    }
}
