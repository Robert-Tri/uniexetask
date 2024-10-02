using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        public IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRolesList()
        {
            var rolesList = await _roleService.GetAllRole();
            if (rolesList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<Role>> response = new ApiResponse<IEnumerable<Role>>();
            response.Data = rolesList;
            return Ok(response);
        }
    }
}
