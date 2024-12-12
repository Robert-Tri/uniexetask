using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;
using uniexetask.services;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/timeline")]
    [ApiController]
    public class TimeLineController : ControllerBase
    {
        private readonly ITimeLineService _timeLineService;
        private readonly IProjectService _projectService;
        private readonly IGroupService _groupService;

        public TimeLineController(ITimeLineService timeLineService, IProjectService projectService)
        {
            _timeLineService = timeLineService;
            _projectService = projectService;
        }

        [Authorize(Policy = "CanViewTimeline")]
        [HttpGet]
        public async Task<IActionResult> GetTimeLines()
        {
            var timeLines = await _timeLineService.GetTimeLines();
            ApiResponse<IEnumerable<Timeline>> response = new ApiResponse<IEnumerable<Timeline>>();
            response.Data = timeLines;
            return Ok(response);
        }

        /*[HttpPost]
        public async Task<IActionResult> CreateTimeLine(Timeline timeLine)
        {
            await _timeLineService.CreateTimeLine(timeLine);
            ApiResponse<Timeline> response = new ApiResponse<Timeline>();
            response.Data = timeLine;
            return Ok(response);
        }*/
        [Authorize(Policy = "CanEditTimeline")]
        [HttpPut("updatemaintimeline")]
        public async Task<IActionResult> UpdateMainTimeLine(MainTimelineUpdateModel timeLine)
        {
            await _timeLineService.UpdateMainTimeLine(timeLine.StartDate, timeLine.EndDate, timeLine.SubjectId);
            ApiResponse<Timeline> response = new ApiResponse<Timeline>();
            return Ok();
        }
        [Authorize(Policy = "CanEditTimeline")]
        [HttpPut("updatespecifictimeline")]
        public async Task<IActionResult> UpdateSpecificTimeline(SpecificTimelineUpdateModel timeline)
        {
            bool result = await _timeLineService.UpdateSpecificTimeLine(timeline.TimelineId, timeline.StartDate, timeline.EndDate, timeline.SubjectId);
            ApiResponse<SpecificTimelineUpdateModel> response = new ApiResponse<SpecificTimelineUpdateModel>();
            if (result)
            {
                response.Data = timeline;
                return Ok();
            }  
            else
            {
                response.Success = false;
                response.Data = timeline;
                response.ErrorMessage = "The specified timeline update is invalid due to date constraints.";
                return BadRequest(response);
            }
        }

        [HttpDelete("{timeLineId}")]
        public IActionResult DeleteTimeLine(int timeLineId)
        {
            _timeLineService.DeleteTimeLine(timeLineId);
            return NoContent();
        }
        [Authorize(Roles = nameof(EnumRole.Manager))]
        [HttpPut("activateenddurationtimeline")]
        public async Task<IActionResult> ActivateEndDuration()
        {
            await _projectService.UpdateEndDurationEXE101();
            await _projectService.UpdateEndDurationEXE201();
            return Ok();
        }
    }
}
