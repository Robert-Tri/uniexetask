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
    class GroupMemberRepsitory : GenericRepository<GroupMember>, IGroupMemberRepository
    {
        public GroupMemberRepsitory(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async System.Threading.Tasks.Task DeleteGroupMembers(int groupId)
        {
            var groupMembers = await dbSet.Where(gm => gm.GroupId == groupId).ToListAsync();
            foreach (var groupMember in groupMembers) 
            {
                dbSet.Remove(groupMember);
            }
        }

        public async Task<int> GetGroupIdByStudentId(int studentId)
        {
            var groupMember = await dbSet.FirstOrDefaultAsync(g => g.StudentId == studentId && g.Group.IsCurrentPeriod);

            if (groupMember == null)
            {
                return -1;
            }

            return groupMember.GroupId;
        }

        public async Task<IEnumerable<GroupMember>> GetGroupMembersWithStudentAndUser(int groupId)
        {
            return await dbSet.Where(gm => gm.GroupId == groupId && gm.Group.IsCurrentPeriod).Include(gm => gm.Student).ThenInclude(gm => gm.User).ToListAsync();
        }
    }
}
