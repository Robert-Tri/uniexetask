using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/campus")]
    [ApiController]
    public class CampusController : ControllerBase
    {
        public ICampusService _campusService;

        public CampusController(ICampusService campusService)
        {
            _campusService = campusService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCampusesList()
        {
            var campusesList = await _campusService.GetAllCampus();
            if (campusesList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<Campus>> response = new ApiResponse<IEnumerable<Campus>>();
            response.Data = campusesList;
            return Ok(response);
        }
    }
}
