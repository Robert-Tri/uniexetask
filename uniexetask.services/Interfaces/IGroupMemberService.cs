using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IGroupMemberService
    {
        Task<IEnumerable<GroupMember>> GetAllGroupMember();
        Task<bool> AddMember(GroupMember member);
        Task<List<User>> GetUsersByGroupId(int groupId);
        Task<GroupMember?> GetGroupByStudentId(int studentId);
        Task<bool> CheckIfStudentInGroup(int studentId);
    }
}
