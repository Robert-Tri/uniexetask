using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IGroupMemberService _groupMemberService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RoleController(IGroupMemberService groupMemberService, IRoleService roleService, IMapper mapper)
        {
            _groupMemberService = groupMemberService;
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
                return NotFound();
            }
            ApiResponse<Role> response = new ApiResponse<Role>
            {
                Data = role
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetRoleByUserId")]
        public async Task<IActionResult> GetRoleByUserId()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var role = await _groupMemberService.GetRoleByUserId(userId);

            ApiResponse<object> response = new ApiResponse<object>
            {
                Data = role
            };
            return Ok(response);
        }

    }
}
