using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/mentor")]
    [ApiController]
    public class MentorController : ControllerBase
    {
        private readonly IMentorService _mentorService;
        private readonly IGroupService _groupService;
        private readonly IConfigSystemService _configSystemService;
        public MentorController(IMentorService mentorService, IGroupService groupService, IConfigSystemService configSystemService)
        {
            _mentorService = mentorService;
            _groupService = groupService;
            _configSystemService = configSystemService;
        }
        [HttpGet]
        public async Task<IActionResult> GetMentors()
        {
            ApiResponse<IEnumerable<Mentor>> respone = new ApiResponse<IEnumerable<Mentor>>();
            respone.Data = await _mentorService.GetMentorsAsync();
            return Ok(respone);
        }
        [HttpGet("getmentorsbycampusid")]
        public async Task<IActionResult> GetMentorsByCampusId(int campusId)
        {
            ApiResponse<IEnumerable<Mentor>> response = new ApiResponse<IEnumerable<Mentor>>();
            response.Data = await _mentorService.GetMentorByCampusId(campusId);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> AssignMentorToGroup(int groupId, int mentorId)
        {
            var configSystem = await _configSystemService.GetConfigSystemById((int)ConfigSystemName.EDIT_MENTOR);
            if (DateTime.Today > configSystem.StartDate)
                return StatusCode(403, "Editing mentors is not allowed after the specified date.");
            await _groupService.AddMentorToGroup(groupId, mentorId);
            return Ok();
        }
    }
}
