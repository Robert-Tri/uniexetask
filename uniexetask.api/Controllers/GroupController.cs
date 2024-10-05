using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
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

        [HttpGet]
        public async Task<IActionResult> GetProjectList()
        {
            var groupsList = await _groupService.GetAllGroups();
            if (groupsList == null)
            {
                return NotFound();
            }
            List<GroupListModel> groups = new List<GroupListModel>();
            foreach (var group in groupsList)
            {
                if (group != null) groups.Add(new GroupListModel
                {
                    GroupId = group.GroupId,
                    GroupName = group.GroupName,
                    SubjectName = group.Subject.SubjectName,
                    HasMentor = group.HasMentor
                });
            }
            ApiResponse<IEnumerable<GroupListModel>> response = new ApiResponse<IEnumerable<GroupListModel>>();
            response.Data = groups;
            return Ok(response);
        }

        [HttpPost("addmentortogroupautomatically")]
        public async Task<IActionResult> AddMentorToGroupAutomatically()
        {
            var groups = (await _groupService.GetGroupsAsync()).ToList();
            var mentors = await _mentorService.GetMentorsAsync();
            int totalGroups = groups.Count();
            int totalMentors = mentors.Count();
            int average = totalGroups / totalMentors;
            int remainder = totalGroups % totalMentors; 
            int groupIndex = 0;

            foreach (var mentor in mentors)
            {
                int groupsToAssign = average + (remainder > 0 ? 1 : 0);

                for (int i = 0; i < groupsToAssign; i++)
                {
                    if (groupIndex >= totalGroups)
                        break;

                    var group = groups[groupIndex];
                    await _groupService.AddMentorToGroup(group.GroupId, mentor.MentorId);
                    groupIndex++;
                }
                if (remainder > 0)
                {
                    remainder--;
                }
            }
            return Ok();
        }
    }
}
