using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class RoleService : IRoleService
    {
        public IUnitOfWork _unitOfWork;
        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Role>> GetAllRole()
        {
            var roleList = await _unitOfWork.Roles.GetAsync();
            return roleList;
        }
    }
}
