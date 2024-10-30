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

        [HttpPost]
        public async Task<IActionResult> CreateTimeLine(Timeline timeLine)
        {
            await _timeLineService.CreateTimeLine(timeLine);
            ApiResponse<Timeline> response = new ApiResponse<Timeline>();
            response.Data = timeLine;
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTimeLine(TimelineUpdateModel timeLine)
        {
            await _timeLineService.UpdateTimeLine(timeLine.StartDate, timeLine.SubjectId);
            ApiResponse<Timeline> response = new ApiResponse<Timeline>();
            return Ok();
        }

        [HttpDelete("{timeLineId}")]
        public IActionResult DeleteTimeLine(int timeLineId)
        {
            _timeLineService.DeleteTimeLine(timeLineId);
            return NoContent();
        }
    }

}
