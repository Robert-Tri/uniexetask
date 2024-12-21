using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;
using System.Security.Claims;

namespace uniexetask.api.Controllers
{
    [Authorize(Roles = nameof(EnumRole.Admin))]
    [Route("api/role-permission")]
    [ApiController]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IMapper _mapper;


        public RolePermissionController(IRolePermissionService rolePermissionService, IMapper mapper)
        {
            _rolePermissionService = rolePermissionService;
            _mapper = mapper;
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            ApiResponse<IEnumerable<string>> response = new ApiResponse<IEnumerable<string>>();
            try
            {
                var roles = await _rolePermissionService.GetRoles();

                if (roles == null) throw new Exception("Roles not found");

                response.Data = roles;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }
        [HttpGet("permissions")]
        public async Task<IActionResult> GetPermissionsByRole(string role)
        {
            ApiResponse<RolePermissionModel> response = new ApiResponse<RolePermissionModel>();
            try
            {
                var features = await _rolePermissionService.GetFeatures();
                if (features == null) throw new Exception("Feature not found.");

                var permissions = await _rolePermissionService.GetPermissions();
                if (permissions == null) throw new Exception("Permission not found.");

                var rolePermissions = await _rolePermissionService.GetRolePermissionsByRole(role);
                if (rolePermissions == null) throw new Exception("Role Permission not found.");
                RolePermissionModel model = new RolePermissionModel
                {
                    Features = features,
                    Permissions = permissions,
                    PermissionsWithRole = rolePermissions
                };
                response.Data = model;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateRolePermissions([FromBody] UpdateRolePermissionsModel request)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid User Id");
                }
                if (request == null || string.IsNullOrEmpty(request.RoleName) || request.Permissions == null)
                {
                    throw new Exception("Invalid request data.");
                }

                var result = await _rolePermissionService.UpdateRolePermissionsAsync(userId, request.RoleName, request.Permissions);

                if (result)
                {
                    response.Data = "Permissions updated successfully.";
                    return Ok(response);
                }
                else
                {
                    throw new Exception("Failed to update permissions.");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }
    }
}
