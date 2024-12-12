using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/meeting-schedules")]
    [ApiController]
    public class MeetingScheduleController : ControllerBase
    {
        private readonly IMeetingScheduleService _meetingScheduleService;

        public MeetingScheduleController(
            IMeetingScheduleService meetingScheduleService)
        {
            _meetingScheduleService = meetingScheduleService;
        }

        [Authorize(Policy = "CanViewMeetingSchedule")]
        [HttpGet("events")]
        public async Task<IActionResult> GetMeetingSchedules()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            ApiResponse<IEnumerable<MeetingScheduleResponse>> response = new ApiResponse<IEnumerable<MeetingScheduleResponse>>();
            try
            {
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid User Id");
                }
                if (string.IsNullOrEmpty(role))
                {
                    throw new Exception("Invalid Role");
                }
                var meetingSchedules = await _meetingScheduleService.GetMeetingSchedules(userId, role);
                response.Data = meetingSchedules;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }

        [Authorize(Policy = "CanCreateMeetingSchedule")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateMeetingSchedule([FromBody] MeetingScheduleModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            ApiResponse<MeetingScheduleResponse> response = new ApiResponse<MeetingScheduleResponse>();
            try
            {
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid User Id");
                }
                if (role != nameof(EnumRole.Mentor)) throw new Exception("You are not a mentor.");
                var newMeetingSchedule = await _meetingScheduleService.CreateMeetingSchedule(userId, model);
                response.Data = newMeetingSchedule;
                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }

        [Authorize(Policy = "CanEditMeetingSchedule")]
        [HttpPost("edit/{meetingScheduleIdStr}")]
        public async Task<IActionResult> EditMeetingSchedule(string meetingScheduleIdStr, [FromBody] MeetingScheduleModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                if (string.IsNullOrEmpty(meetingScheduleIdStr) || !int.TryParse(meetingScheduleIdStr, out int meetingScheduleId))
                {
                    throw new Exception("Invalid Meeting schedule Id");
                }
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid User Id");
                }
                var result = await _meetingScheduleService.EditMeetingSchedule(userId, meetingScheduleId, model);
                if (result)
                {
                    response.Data = "Meeting updated successfully!";
                    return Ok(response);
                }
                throw new Exception("Update meeting failed.");

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }

        [Authorize(Policy = "CanDeleteMeetingSchedule")]
        [HttpDelete("delete/{meetingScheduleIdStr}")]
        public async Task<IActionResult> DeleteMeetingSchedule(string meetingScheduleIdStr)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                if (string.IsNullOrEmpty(meetingScheduleIdStr) || !int.TryParse(meetingScheduleIdStr, out int meetingScheduleId))
                {
                    throw new Exception("Invalid Meeting Schedule Id");
                }
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid User Id");
                }
                var result = await _meetingScheduleService.DeleteMeetingSchedule(userId, meetingScheduleId);
                if (result)
                {
                    response.Data = "Meeting deleted successfully!";
                    return Ok(response);
                }
                throw new Exception("Delete meeting failed.");

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }
    }
}
