using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private IGroupService _groupService;
        private IMentorService _mentorService;
        public GroupController(IGroupService groupService, IMentorService mentorService)
        {
            _groupService = groupService;
            _mentorService = mentorService;
        }
        [HttpPost("addmentortogroupautomatically")]
        public async Task<IActionResult> AddMentorToGroupAutomatically()
        {
            var groups = (await _groupService.GetGroupsAsync()).ToList();
            var mentors = await _mentorService.GetMentorsAsync();
            int average = groups.Count() / mentors.Count();
            int groupIndex = 0;
            foreach (var mentor in mentors)
            {
                for (int i = 0; i < average; i++)
                {
                    if (groupIndex >= groups.Count())
                        break;

                    var group = groups[groupIndex];
                    await _groupService.AddMentorToGroup(group.GroupId, mentor.MentorId);
                    groupIndex++;
                }
            }
            return Ok();
        }
    }
}
