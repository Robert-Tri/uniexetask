using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IGroupMemberRepository : IGenericRepository<GroupMember>
    {
       Task<int> GetGroupIdByStudentId(int studentId);
       System.Threading.Tasks.Task DeleteGroupMembers(int groupId);
    }
}
