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
        public IUnitOfWork _unitOfWork;

        public RolePermissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            var role = await _unitOfWork.Roles.GetRoleByNameAsync(roleName);
            if (role == null) return null;

            var roleWithPermissions = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(role.RoleId);
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

        public async Task<bool> UpdateRolePermissionsAsync(int userId, string roleName, List<int> permissionIds)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var role = await _unitOfWork.Roles.GetRoleByNameAsync(roleName);
                if (role == null) return false;

                var roleWithPermission = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(role.RoleId);
                if (roleWithPermission == null) return false;

                var permissions = roleWithPermission.Permissions.ToList();
                foreach (var permission in permissions)
                {
                    roleWithPermission.Permissions.Remove(permission);
                }
                _unitOfWork.Roles.Update(roleWithPermission);
                _unitOfWork.Save();
                foreach (var permissionId in permissionIds)
                {
                    var permission = await _unitOfWork.Permissions.GetByIDAsync(permissionId);
                    if (permission == null) return false;
                    roleWithPermission.Permissions.Add(permission);
                }
                _unitOfWork.Roles.Update(roleWithPermission);
                var refreshTokens = await _unitOfWork.RefreshTokens.GetAllActiveRefreshTokens();
                foreach (var token in refreshTokens)
                {
                    if (token.UserId == userId) continue;
                    token.Status = false;
                    token.Revoked = DateTime.Now;
                    _unitOfWork.RefreshTokens.Update(token);
                }
                _unitOfWork.Save();
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw new Exception(ex.Message);
            }
        }
    }
}
