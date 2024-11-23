using uniexetask.core.Models;
using uniexetask.core.Models.Enums;

namespace uniexetask.services.Interfaces
{
    public interface IGroupService
    {
        Task<IEnumerable<object>> GetApprovedGroupsAsync();
        Task<IEnumerable<Group>> GetGroupAndSubject();
        Task<IEnumerable<Group>> GetGroupsAsync();
        Task<IEnumerable<Group>> GetAllGroup();
        Task<bool> CreateGroup(Group group);
        Task<Group> GetGroupById(int id);
        Task<Group> GetGroupWithTopic(int groupId);
        System.Threading.Tasks.Task AddMentorToGroup(int groupId, int mentorId);
        System.Threading.Tasks.Task AddMentorToGroupAutomatically();
        Task<Group?> GetGroupWithSubject(int groupId);
        System.Threading.Tasks.Task UpdateAndAssignStudentsToGroups(SubjectType subjectType);
        Task<bool> UpdateGroupApproved(int groupId);
    }
}
