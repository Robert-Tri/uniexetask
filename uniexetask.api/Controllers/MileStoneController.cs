using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;
using System.Security.Claims;
using uniexetask.services;
using Microsoft.AspNetCore.Authorization;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MileStoneController : ControllerBase
    {
        private readonly IMilestoneService _milestoneService;
        private readonly ITimeLineService _timeLineService;

        public MileStoneController(IMilestoneService milestoneService, ITimeLineService timeLineService)
        {
            _milestoneService = milestoneService;
            _timeLineService = timeLineService;
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

        [HttpGet("getundeletemilestonesbysubjectid")]
        public async Task<IActionResult> GetUndeleteMileStonesBySubjectId(int subjectId)
        {
            ApiResponse<IEnumerable<Milestone>> respone = new ApiResponse<IEnumerable<Milestone>>();
            respone.Data = await _milestoneService.GetUndeleteMileStonesBySubjectId(subjectId);
            return Ok(respone);
        }
        [HttpGet("getundeletemilestonebyid")]
        public async Task<IActionResult> GetUndeleteMileStone(int id)
        {
            ApiResponse<Milestone> response = new ApiResponse<Milestone>();
            response.Data = await _milestoneService.GetUndeleteMilestoneWithCriteria(id);
            return Ok(response);
        }

        [HttpPost]
        [Route("upload-excel")]
        public async Task<IActionResult> CreateMilestoneWithCriteria(IFormFile excelFile)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                if (excelFile == null || excelFile.Length == 0)
                {
                    throw new Exception("File is not selected or is empty.");
                }
                var timeline = await _timeLineService.GetTimelineById(7); // 7 là Timeline Update Milestone With Criteria
                if(timeline.StartDate < DateTime.Now && timeline.EndDate > DateTime.Now)
                {
                    var importMilestoneWithCriteria = await _milestoneService.ImportMilestoneWithCriteriaFromExcel(excelFile);
                    if (importMilestoneWithCriteria)
                    {
                        response.Data = "All Milestone With Criteria were successfully created.";
                        return Ok(response);
                    }
                    else throw new Exception("Milestones with Criteria were failed to create.");
                }
                else throw new Exception("Timeline not available to Update Milestone With Criteria.");
            }
            catch (Exception e)
            {
                response.Success = false;
                response.ErrorMessage = e.Message;
                return BadRequest(response);
            }
        }
    }
}
