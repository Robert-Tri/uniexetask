using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IGroupService
    {
        Task<IEnumerable<Group>> GetGroupsAsync();
        Task<Group> GetGroupById(int id);
        System.Threading.Tasks.Task AddMentorToGroup(int groupId, int mentorId);
    }
}
