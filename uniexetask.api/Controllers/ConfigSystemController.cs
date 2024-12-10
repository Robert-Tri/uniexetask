using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/configsystem")]
    [ApiController]
    public class ConfigSystemController : ControllerBase
    {
        private IConfigSystemService _configSystemService;
        public ConfigSystemController(IConfigSystemService configSystemService)
        {
            _configSystemService = configSystemService;
        }
        [Authorize(Roles = nameof(EnumRole.Manager))]
        [HttpGet]
        public async Task<IActionResult> GetConfigSystems() 
        {
            ApiResponse<IEnumerable<ConfigSystem>> respone = new ApiResponse<IEnumerable<ConfigSystem>>();
            respone.Data = await _configSystemService.GetConfigSystems();
            return Ok(respone);
        }
        [Authorize(Roles = nameof(EnumRole.Manager))]
        [HttpPut]
        public async Task<IActionResult> EditConfigSystem(EditConfigSystemModel editConfigSystemModel)
        {
            bool result = await _configSystemService.UpdateConfigSystem(editConfigSystemModel.ConfigSystemId, editConfigSystemModel.Number, editConfigSystemModel.StartDate);
            ApiResponse<bool> response = new ApiResponse<bool>();
            response.Data = result;
            return Ok(response); 
        }
    }
}
