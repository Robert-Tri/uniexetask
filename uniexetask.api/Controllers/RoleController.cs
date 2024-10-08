using AutoMapper;
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
        private readonly IMapper _mapper;

        public RoleController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
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

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById(int roleId)
        {
            var role = await _roleService.GetRoleById(roleId);
            if (role == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy role
            }
            ApiResponse<Role> response = new ApiResponse<Role>
            {
                Data = role
            };
            return Ok(response); // Trả về role nếu tìm thấy
        }

    }
}
