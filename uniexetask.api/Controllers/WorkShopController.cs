using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Extensions;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/workshop")]
    [ApiController]
    public class WorkShopController : ControllerBase
    {
        private readonly IWorkShopService _workShopService;
        private readonly IEmailService _emailService;
        public WorkShopController(IWorkShopService workShopService, IEmailService emailService)
        {
            _workShopService = workShopService;
            _emailService = emailService;
        }
        [HttpGet]
        public async Task<IActionResult> GetWorkShops()
        {
            var workShops = await _workShopService.GetWorkShops();
            ApiResponse<IEnumerable<Workshop>> respone = new ApiResponse<IEnumerable<Workshop>>();
            respone.Data = workShops;
            return Ok(respone);
        }
        [HttpPost]
        public async Task<IActionResult> CreateWorkShop(Workshop workshop)
        {
            await _workShopService.CreateWorkShop(workshop);
            ApiResponse<Workshop> respone = new ApiResponse<Workshop>();

            respone.Data = workshop;
            return Ok(respone);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateWorkShop(Workshop workShop)
        {
            await _workShopService.UpdateWorkShop(workShop);
            ApiResponse<Workshop> respone = new ApiResponse<Workshop>();
            respone.Data = workShop;
            return Ok(respone);
        }
        [HttpDelete("{workShopId}")]
        public IActionResult DeleteWorkShop(int workShopId)
        {
            _workShopService.DeleteWorkShop(workShopId);
            return NoContent();
        }
    }
}
