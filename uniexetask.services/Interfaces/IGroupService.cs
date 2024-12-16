using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using static uniexetask.services.GroupService;

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
        Task<Group?> GetGroupByUserId(int userId);
        Task<Group> GetGroupWithTopic(int groupId);
        System.Threading.Tasks.Task AddMentorToGroup(int groupId, int mentorId);
        System.Threading.Tasks.Task AddMentorToGroupAutomatically();
        Task<Group?> GetGroupWithSubject(int groupId);
        System.Threading.Tasks.Task UpdateAndAssignStudentsToGroups(SubjectType subjectType);
        Task<bool> UpdateGroupApproved(int groupId);
        Task<IEnumerable<Group>> SearchGroupsByGroupNameAsync(int userId, string query);
        Task<IEnumerable<GroupDetailsResponseModel>> GetCurrentGroupsWithMembersAndMentors();
        Task<bool> DeleteGroup(int groupId);
        Task<bool> UpdateGroupName(string name, int groupId);
        Task<bool> AddStudentToGroup(int groupId, int studentId);
    }
}
