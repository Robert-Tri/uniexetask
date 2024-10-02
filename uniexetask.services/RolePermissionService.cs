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
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRoleRepository _roleRepository;
        public IUnitOfWork _unitOfWork;

        public RolePermissionService(IUnitOfWork unitOfWork, IRoleRepository roleRepository)
        {
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<Feature>> GetFeatures()
        {
            var features = await _unitOfWork.Features.GetAsync();
            return features;
        }

        public async Task<IEnumerable<Permission>> GetPermissions()
        {
            var permissions = await _unitOfWork.Permissions.GetAsync();
            return permissions;
        }

        public async Task<IEnumerable<Permission>?> GetRolePermissionsByRole(string roleName)
        {
            var role = await _roleRepository.GetRoleByNameAsync(roleName);
            if (role == null) return null;

            var roleWithPermissions = await _roleRepository.GetRoleWithPermissionsAsync(role.RoleId);
            if (roleWithPermissions == null) return null;
            else
            {
                return roleWithPermissions.Permissions;
            }
        }

        public async Task<IEnumerable<string>?> GetRoles()
        {
            var roles = await _unitOfWork.Roles.GetAsync();
            List<string> result = new List<string>();
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    result.Add(role.Name);
                }
                return result;
            }
            return null;
        }
    }
}
