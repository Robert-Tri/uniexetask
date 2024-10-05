using Microsoft.AspNetCore.Mvc;
﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
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

        [HttpGet("getapprovedgroup")]
        public async Task<IEnumerable<object>> GetApprovedGroup()
        {
            var groups = await _groupService.GetApprovedGroupsAsync();
            return groups;
        }

        [HttpPost("addmentortogroup")]
        public async Task<IActionResult> AddMentorToGroup(int groupId, int mentorId)
        {
            await _groupService.AddMentorToGroup(groupId, mentorId);
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> GetGroupMemberList()
        {
            var groupList = await _groupService.GetAllGroup();
            if (groupList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<Group>> response = new ApiResponse<IEnumerable<Group>>();
            response.Data = groupList;
            return Ok(response);
        }

        [HttpPost("addmentortogroupautomatically")]
        public async Task<IActionResult> AddMentorToGroupAutomatically()
        {
            var groups = (IEnumerable<dynamic>)(await _groupService.GetApprovedGroupsAsync());
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

                    var group = groups.ElementAt(groupIndex);
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
