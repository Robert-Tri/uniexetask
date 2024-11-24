using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MileStoneController : ControllerBase
    {
        private readonly IMilestoneService _milestoneService;
        public MileStoneController(IMilestoneService milestoneService)
        {
            _milestoneService = milestoneService;
        }
        [HttpGet]
        public async Task<IActionResult> GetMileStones()
        {
            ApiResponse<IEnumerable<Milestone>> respone = new ApiResponse<IEnumerable<Milestone>>();
            respone.Data = await _milestoneService.GetMileStones();
            return Ok(respone);
        }
        [HttpGet("getmilestonesbysubjectid")] 
        public async Task<IActionResult> GetMileStonesBySubjectId(int subjectId)
        {
            ApiResponse<IEnumerable<Milestone>> respone = new ApiResponse<IEnumerable<Milestone>>();
            respone.Data = await _milestoneService.GetMileStonesBySubjectId(subjectId);
            return Ok(respone);
        }
        [HttpGet("getmilestonebyid")]
        public async Task<IActionResult> GetMileStone(int id)
        {
            ApiResponse<Milestone> response = new ApiResponse<Milestone>();
            response.Data = await _milestoneService.GetMilestoneWithCriteria(id);
            return Ok(response);
        }
    }
}
