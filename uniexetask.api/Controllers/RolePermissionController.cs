using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    //[Authorize]
    [Route("api/role-permission")]
    [ApiController]
    public class RolePermissionController : ControllerBase
    {
        public readonly IRolePermissionService _rolePermissionService;
        public RolePermissionController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        //[Authorize(Roles = "1")]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _rolePermissionService.GetRoles();
            if (roles != null)
            {
                return Ok(roles);
            }
            else return NotFound();
        }
        [HttpGet("permissions")]
        public async Task<IActionResult> GetPermissionsByRole(string role)
        {
            ApiResponse<RolePermissionModel> response = new ApiResponse<RolePermissionModel>();
            var features = await _rolePermissionService.GetFeatures();
            if (features == null) return NotFound();
            

            var permissions = await _rolePermissionService.GetPermissions();
            if (permissions == null) return NotFound();

            var rolePermissions = await _rolePermissionService.GetRolePermissionsByRole(role);
            if (rolePermissions == null) return NotFound();
            RolePermissionModel model = new RolePermissionModel
            {
                Features = features,
                Permissions = permissions,
                PermissionsWithRole = rolePermissions
            };
            response.Data = model;
            return Ok(response);
        }
    }
}
