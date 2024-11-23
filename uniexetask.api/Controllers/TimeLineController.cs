using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/timeline")]
    [ApiController]
    public class TimeLineController : ControllerBase
    {
        private readonly ITimeLineService _timeLineService;

        public TimeLineController(ITimeLineService timeLineService)
        {
            _timeLineService = timeLineService;
        }

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

        [HttpPut("updatemaintimeline")]
        public async Task<IActionResult> UpdateMainTimeLine(MainTimelineUpdateModel timeLine)
        {
            await _timeLineService.UpdateMainTimeLine(timeLine.StartDate, timeLine.SubjectId);
            ApiResponse<Timeline> response = new ApiResponse<Timeline>();
            return Ok();
        }
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
    }
}
