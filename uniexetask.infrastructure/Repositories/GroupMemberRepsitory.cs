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
            var groupMembers = await dbSet.Where(gm => gm.GroupId == gm.GroupId).ToListAsync();
            foreach (var groupMember in groupMembers) 
            {
                dbSet.Remove(groupMember);
            }
        }

        public async Task<int?> GetGroupIdByStudentId(int studentId)
        {
            return (await dbSet.FirstOrDefaultAsync(g => g.StudentId == studentId))?.GroupId;
        }
    }
}
