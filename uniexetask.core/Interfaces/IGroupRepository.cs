using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        Task<Group?> GetGroupWithProjectAsync(int groupId);
        Task<Group?> GetGroupWithSubjectAsync(int groupId);
        Task<IEnumerable<Group>> GetHasNoMentorGroupsWithGroupMembersAndStudent();
        Task<IEnumerable<Group>> GetApprovedGroupsWithGroupMembersAndStudent();
    }
}
