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

        public async Task<Group?> GetGroupWithProjectAsync(int groupId)
        {
            var group = await _unitOfWork.Groups.GetGroupWithProjectAsync(groupId);
            return group;
        }
    }
}
