using Microsoft.AspNetCore.Mvc;
﻿using Microsoft.AspNetCore.Http;
﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;
using System;
using System.Security.Claims;
using uniexetask.core.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static uniexetask.services.GroupService;

namespace uniexetask.api.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IMentorService _mentorService;
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        public GroupController(IGroupService groupService, IMentorService mentorService, IMapper mapper, IStudentService studentService)
        {
            _groupService = groupService;
            _mentorService = mentorService;
            _mapper = mapper;
            _studentService = studentService;
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
                    SubjectCode = group.Subject.SubjectCode,
                    HasMentor = group.HasMentor,
                    Status = group.Status
                });
            }
            ApiResponse<IEnumerable<GroupListModel>> response = new ApiResponse<IEnumerable<GroupListModel>>();
            response.Data = groups;
            return Ok(response);
        }

        [Authorize]
        [HttpGet("search-group")]
        public async Task<IActionResult> SearchGroupByGroupName([FromQuery] string query = "")
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<IEnumerable<GroupModel>> response = new ApiResponse<IEnumerable<GroupModel>>();
            try
            {
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid UserId");
                }
                List<GroupModel> groupList = new List<GroupModel>();
                var groups = await _groupService.SearchGroupsByGroupNameAsync(userId, query);

                if (groups.Count() == 0)
                {
                    throw new Exception("No matching group name found.");
                }

                foreach (var group in groups)
                {
                    groupList.Add(_mapper.Map<GroupModel>(group));
                }
                response.Data = groupList;

                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.ErrorMessage = e.Message;
                return Ok(response);
            }
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
            await _groupService.AddMentorToGroupAutomatically();

            return Ok();
        }
        [HttpGet("/subject/{groupIdStr}")]
        public async Task<IActionResult> GétubjectInGroup(string groupIdStr)
        {
            if (string.IsNullOrEmpty(groupIdStr) || !int.TryParse(groupIdStr, out int groupId))
            {
                return NotFound();
            }
            var group = await _groupService.GetGroupWithSubject(groupId);
            if (group == null)
            {
                return NotFound();
            }
            ApiResponse<SubjectModel> response = new ApiResponse<SubjectModel>();
            SubjectModel model = _mapper.Map<SubjectModel>(group.Subject);

            response.Data = model;
            return Ok(response);
        }

        [Authorize]
        [HttpGet("get-group")]
        public async Task<IActionResult> GetGroupToPrepareAddMenberInChatGroup()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<GroupModel> response = new ApiResponse<GroupModel>();
            try 
            {
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid UserId");
                }
                var group = await _groupService.GetGroupByUserId(userId);
                if (group != null)
                {
                    foreach (var member in group.GroupMembers)
                    {
                        var student = await _studentService.GetStudentByUserId(userId);
                        if (student == null) continue;
                        if (student.StudentId == member.StudentId && member.Role == nameof(GroupMemberRole.Leader))
                        {
                            response.Data = _mapper.Map<GroupModel>(group);
                            return Ok(response);
                        }
                        
                    }
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }
        [Authorize(Roles = nameof(EnumRole.Manager))]
        [HttpGet("getcurrentgroupswithmembersandMentor")]
        public async Task<IActionResult> GetCurrentGroupsWithMemberAndMentor()
        {
            ApiResponse<IEnumerable<GroupDetailsResponseModel>> respone = new ApiResponse<IEnumerable<GroupDetailsResponseModel>>();
            respone.Data = await _groupService.GetCurrentGroupsWithMembersAndMentors();
            return Ok(respone);
        }
        [Authorize(Roles = nameof(EnumRole.Manager))]
        [HttpPost("assignmentortogroupautomatically")]
        public async Task<IActionResult> AssignMentorToGroupAutomatically()
        {
            await _groupService.AddMentorToGroupAutomatically();
            return Ok();
        }
        [Authorize(Roles = nameof(EnumRole.Manager))]
        [HttpPost("assigngroupsautomatically")]
        public async Task<IActionResult> AssignGroupsAutomatically()
        {
            await _groupService.UpdateAndAssignStudentsToGroups(SubjectType.EXE101);
            await _groupService.UpdateAndAssignStudentsToGroups(SubjectType.EXE201);
            return Ok();
        }
        [Authorize(Roles = nameof(EnumRole.Manager))]
        [HttpPost("addstudenttogroup")]
        public async Task<IActionResult> AddStudentToGroup(int groupId, string studentCode)
        {
            try
            {
                var student = await _studentService.GetStudentByCode(studentCode);
                if(student == null) 
                    return NotFound(new ApiResponse<Student> { Success = false, ErrorMessage = "Student Not Found"});
                var isAdded = await _groupService.AddStudentToGroup(groupId, student.StudentId);
                if(isAdded)
                    return Ok(new ApiResponse<bool> { Success = true, Data = isAdded });
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<Group> { Success = false, ErrorMessage = ex.Message});
            }
        }
    }
}
