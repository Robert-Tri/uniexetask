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

        public async Task<IEnumerable<object>> GetApprovedGroupsAsync()
        {
            var groups = await _unitOfWork.Groups.GetAsync(
                filter: g => g.Status.Equals("Status 1"),
                includeProperties: "GroupMembers.Student"
            );

            var result = groups.Select(g => new
            {
                g.GroupId,
                g.GroupName, 
                g.Status,
                GroupMembers = g.GroupMembers.Select(m => new
                {
                    m.StudentId,
                    m.Role,
                    Student = new
                    {
                        m.Student.StudentCode,
                        m.Student.Major,
                    }
                })
            });

            return result;
        }

        public async Task<IEnumerable<Group>> GetAllGroup()
        {
            var groupList = await _unitOfWork.Groups.GetAsync();
            return groupList;
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
