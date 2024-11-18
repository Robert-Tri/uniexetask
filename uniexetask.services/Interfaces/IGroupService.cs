using uniexetask.core.Models;

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
        System.Threading.Tasks.Task AddMentorToGroup(int groupId, int mentorId);
        System.Threading.Tasks.Task AddMentorToGroupAutomatically();
        Task<Group?> GetGroupWithSubject(int groupId);
        System.Threading.Tasks.Task UpdateAndAssignStudentsToGroups();
    }
}
