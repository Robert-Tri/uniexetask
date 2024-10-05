using Microsoft.AspNetCore.Mvc;
﻿using Microsoft.AspNetCore.Http;
﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Request;
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
        private readonly IMapper _mapper;
        public GroupController(IGroupService groupService, IMentorService mentorService, IMapper mapper)
        {
            _groupService = groupService;
            _mentorService = mentorService;
            _mapper = mapper;
        }

        [HttpGet("group-subject")]
        public async Task<IActionResult> GetGroupAndSubject()
        {
            var groupsList = await _groupService.GetGroupAndSubject();
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
        public async Task<IActionResult> GetGroupList()
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

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupModel group)
        {

            var obj = _mapper.Map<Group>(group);
            var isGroupCreated = await _groupService.CreateGroup(obj);

            if (isGroupCreated)
            {
                return Ok(isGroupCreated);
            }
            else
            {
                return BadRequest();
            }
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
