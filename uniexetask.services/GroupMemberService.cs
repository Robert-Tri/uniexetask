using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class GroupMemberService : IGroupMemberService
    {
        public IUnitOfWork _unitOfWork;
        public GroupMemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<GroupMember>> GetAllGroupMember()
        {
            var groupList = await _unitOfWork.GroupMembers.GetAsync();
            return groupList;
        }

        public async Task<bool> AddMember(GroupMember members)
        {
            if (members != null)
            {
                await _unitOfWork.GroupMembers.InsertAsync(members);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public async Task<List<User>> GetUsersByGroupId(int groupId)
        {
            // Lấy danh sách các thành viên trong group với GroupId
            var groupMembers = await _unitOfWork.GroupMembers.GetAsync(gm => gm.GroupId == groupId, includeProperties: "Student.User");

            // Lấy User từ mỗi GroupMember
            var users = groupMembers.Select(gm => gm.Student.User).ToList();

            return users;
        }
    }
}
