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

        public async Task<int?> GetGroupIdByStudentId(int studentId)
        {
            return (await dbSet.FirstOrDefaultAsync(g => g.StudentId == studentId))?.GroupId;
        }
    }
}
