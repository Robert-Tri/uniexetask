using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class GroupService: IGroupService
    {
        public IUnitOfWork _unitOfWork;
        public GroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Group>> GetGroupsAsync()
        {
            var groups = await _unitOfWork.Groups.GetAsync(filter: g => g.Status.Equals("status 1"));
            return groups;
        }

        public async Task<IEnumerable<Group>> GetAllGroup()
        {
            var groupList = await _unitOfWork.Groups.GetAsync();
            return groupList;
        }

        public async Task<bool> CreateGroup(Group group)
        {
            if (group != null)
            {
                await _unitOfWork.Groups.InsertAsync(group);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        /*        public async Task<Group?> GetGroupWithProjectAsync(int groupId)
                {
                    var group = await _unitOfWork.Groups.GetGroupWithProjectAsync(groupId);
                    return group;
                }*/

        public async Task<Group> GetGroupById(int id)
        {
            var group = await _unitOfWork.Groups.GetByIDAsync(id);
            return group;
        }

        public async System.Threading.Tasks.Task AddMentorToGroup(int groupId, int mentorId)
        {
            await _unitOfWork.AddMentorToGroup(groupId, mentorId);
        }
    }
}
