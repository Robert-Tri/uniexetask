using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using uniexetask.api.Hubs;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/reqTopic")]
    [ApiController]
    public class ReqTopicController : ControllerBase
    {
        private readonly string _bucketName = "exeunitask.appspot.com";
        private readonly StorageClient _storageClient;
        private readonly IProjectProgressService _projectProgressService;
        private readonly ITimeLineService _timeLineService;
        private readonly IProjectService _projectService;
        private readonly IMentorService _mentorService;
        private readonly IMapper _mapper;
        private readonly ITopicService _topicService;
        private readonly IReqTopicService _reqTopicService;
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ReqTopicController(
            StorageClient storageClient,
            IProjectProgressService projectProgressService,
            ITimeLineService timeLineService,
            IProjectService projectService, 
            ITopicService userService, 
            IReqTopicService reqTopicService, 
            IMentorService mentorService, 
            IMapper mapper, 
            IGroupService groupService, 
            IGroupMemberService groupMemberService,
            IHubContext<NotificationHub> hubContext,
            INotificationService notificationService)
        {
            _projectProgressService = projectProgressService;
            _storageClient = storageClient;
            _projectService = projectService;
            _topicService = userService;
            _mentorService = mentorService;
            _reqTopicService = reqTopicService;
            _mapper = mapper;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _timeLineService = timeLineService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }   

        [HttpGet]
        public async Task<IActionResult> GetReqTopicList()
        {
            var reqTopicList = await _reqTopicService.GetAllReqTopic();
            if (reqTopicList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<RegTopicForm>> response = new ApiResponse<IEnumerable<RegTopicForm>>();
            response.Data = reqTopicList;
            return Ok(response);
        }

        [HttpGet("ReqTopicByMentorId")]
        public async Task<IActionResult> GetReqTopicListByMentorId(int groupId)
        {
            //var mentor = await _mentorService.GetMentorWithGroupAsync(groupId);

            var mentor = await _mentorService.GetMentorByGroupId(groupId);

            var reqTopicList = await _reqTopicService.GetReqTopicByMentorId(mentor.MentorId);

            if (reqTopicList == null)
            {
                return NotFound("Mentor not found");
            }

            return Ok(reqTopicList);
        }



        [Authorize(Roles = "Mentor")]
        [HttpGet("GetGroupReqTopicList")]
        public async Task<IActionResult> GetGroupReqTopicList()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId) || string.IsNullOrEmpty(userRole) || !userRole.Equals("Mentor"))
            {
                return NotFound();
            }

            var mentor = await _mentorService.GetMentorWithGroupAsync(userId);

            if (mentor == null || !mentor.Groups.Any())
            {
                return NotFound("No groups found.");
            }

            var groupIds = mentor.Groups.Select(g => g.GroupId).ToList();

            var groups = new List<object>();

            foreach (var groupId in groupIds)
            {
                var group = await _groupService.GetGroupWithTopic(groupId);
                if (group != null)
                {
                    var regTopicForms = group.RegTopicForms
                        .Where(rt => rt.Status == true)
                        .Select(rt => new
                        {
                            regTopicId = rt.RegTopicId,
                            topicCode = rt.TopicCode,
                            topicName = rt.TopicName,
                            description = rt.Description,
                            status = rt.Status
                        }).ToList();

                    if (regTopicForms.Any()) // Chỉ thêm nhóm nếu regTopicForms không rỗng
                    {
                        var groupData = new
                        {
                            groupId = group.GroupId,
                            groupName = group.GroupName,
                            subjectId = group.SubjectId,
                            hasMentor = group.HasMentor,
                            isCurrentPeriod = group.IsCurrentPeriod,
                            status = group.Status,
                            isDeleted = group.IsDeleted,
                            regTopicForms = regTopicForms,
                            subjectCode = group.Subject.SubjectCode
                        };

                        groups.Add(groupData); // Thêm nhóm vào danh sách
                    }
                }
            }

            if (!groups.Any())
            {
                return NotFound("No groups found.");
            }

            var response = new ApiResponse<IEnumerable<object>>()
            {
                Data = groups
            };

            return Ok(response);
        }


        [Authorize(Roles = "Mentor")]
        [HttpGet("GetReqTopicList/{groupId}")]
        public async Task<IActionResult> GetReqTopicList( int groupId)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId) || string.IsNullOrEmpty(userRole) || !userRole.Equals("Mentor"))
            {
                return Unauthorized("You are not authorized to access this resource.");
            }

            var reqTopics = await _reqTopicService.GetReqTopicByGroupId(groupId);

            if (reqTopics == null || !reqTopics.Any())
            {
                return NotFound("No request topics found for the given group.");
            }

            var response = new ApiResponse<IEnumerable<object>>
            {
                Data = reqTopics,
                Success = true
            };

            return Ok(response);
        }


        [HttpPost("ApproveTopic")]
        public async Task<IActionResult> ApproveTopic([FromBody] ReqTopicModel reqTopic)
        {
            var topic = new TopicModel
            {
                TopicCode = reqTopic.TopicCode,
                TopicName = reqTopic.TopicName,
                Description = reqTopic.Description
            };

            var objTopic = _mapper.Map<Topic>(topic);
            var objReqTopic = _mapper.Map<RegTopicForm>(reqTopic);

            var topicList = await _topicService.GetAllTopics();
            var group = await _groupService.GetGroupById(reqTopic.GroupId);

            if (topicList.Any(t => t.TopicCode == objTopic.TopicCode))
            {
                return BadRequest("Topic with this code already exists.");
            }

            var topicId = await _topicService.CreateTopic(objTopic);

            var timeLines = group.SubjectId == 1
                ? await _timeLineService.GetTimelineById(1)
                : await _timeLineService.GetTimelineById(2);

            var project = new ProjectModel
            {
                GroupId = group.GroupId,
                TopicId = topicId,
                StartDate = DateTime.Now,
                EndDate = timeLines.EndDate, 
                SubjectId = group.SubjectId,
                IsCurrentPeriod = true,
                Status = "In_Progress",
                IsDeleted = false
            };

            var objProject = _mapper.Map<Project>(project);
            var createProject = await _projectService.CreateProject(objProject);

            var projectProgress = new ProjectProgressModel
            {
                ProjectId = createProject.ProjectId,
                ProgressPercentage = 0,
                UpdatedDate = DateTime.Now,
                Note = null,
                IsDeleted= false
            };

            var objProjectProgress = _mapper.Map<ProjectProgress>(projectProgress);
            var createProjectProgress = await _projectProgressService.CreateProjectProgress(objProjectProgress);

            var groupUpdate = await _groupService.UpdateGroupApproved(reqTopic.GroupId);

            var reqTopicList = await _reqTopicService.GetReqTopicByGroupId(reqTopic.GroupId);

            foreach (var reqTopicItem in reqTopicList)
            {
                await _reqTopicService.UpdateApproveTopic(reqTopicItem.RegTopicId);
            }

            if (topicId > 0 && createProject != null)
            {
                var response = new ApiResponse<object>
                {
                    Data = new { Message = "Topic and Project created successfully!", TopicId = topicId }
                };
                return Ok(response);
            }
            else
            {
                return BadRequest("Lỗi Tạo.");
            }
        }

        [Authorize]
        [HttpPost("RejectTopic")]
        public async Task<IActionResult> RejectTopic([FromBody] RejectRegTopicModel reqTopic)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid user id");
                }
                if (reqTopic.RegTopicId <= 0)
                {
                    throw new Exception("Invalid RegTopicId.");
                }

                var result = await _reqTopicService.RejectRegTopicFormAsync(reqTopic.RegTopicId, reqTopic.RejectionReason);

                if (result)
                {
                    var regTopicForm = await _reqTopicService.GetReqTopicById(reqTopic.RegTopicId);
                    if (regTopicForm == null) throw new Exception("Request topic not found");
                    var users = await _groupMemberService.GetUsersByGroupId(regTopicForm.GroupId);
                    foreach (var user in users)
                    {
                        var newNotification = await _notificationService.CreateNotification(userId, user.UserId, $"Your group topic was rejected for the reason: " +
                            $"{(reqTopic.RejectionReason != null ? reqTopic.RejectionReason : "No Reason")}");
                        await _hubContext.Clients.User(user.UserId.ToString()).SendAsync("ReceiveNotification", newNotification);
                    }
                    response.Data = "Topic rejected successfully!";
                    return Ok(response);
                }
                else
                {
                    throw new Exception("Reject topic failed.");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }

        }

        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpPost]
        public async Task<IActionResult> CreateReqTopic(IFormFile file, string topicName)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var role = await _groupMemberService.GetRoleByUserId(userId);

            if (role != "Leader")
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You are not a leader to perform this operation." });
            }

            var groupCheck = await _groupMemberService.GetGroupMemberByUserId(userId);

            var groupMentor = await _groupService.GetGroupById(groupCheck.GroupId);

            if (groupMentor.HasMentor == false )
                return BadRequest("The group does not have a mentor.");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var existedTopics = await _reqTopicService.GetReqTopicByDescription($"Topic{groupCheck.GroupId}/{file.FileName}");
            if (existedTopics.Any())
                return Conflict(new { Message = "Description with the same name already exists." });

            var mentor = await _mentorService.GetMentorByGroupId(groupCheck.GroupId);

            var reqTopicList = await _reqTopicService.GetReqTopicByMentorId(mentor.MentorId);

            var existingTopicName = reqTopicList.Any(rt => string.Equals(rt.TopicName, topicName, StringComparison.OrdinalIgnoreCase));
            if (existingTopicName)
            {
                return Conflict(new { Message = "Topic with the same name already exists." });
            }

            var topicCodeMax = await _reqTopicService.GetMaxTopicCode();
            int nextCodeNumber = 1;
            if (!string.IsNullOrEmpty(topicCodeMax) && topicCodeMax.Length > 2)
            {
                var numericPart = topicCodeMax.Substring(2);
                if (int.TryParse(numericPart, out int currentMax))
                {
                    nextCodeNumber = currentMax + 1;
                }
            }

            var topicCode = $"TP{nextCodeNumber:D3}";

            var groupMember = await _groupMemberService.GetGroupMemberByUserId(userId);
            var group = await _groupService.GetGroupById(groupMember.GroupId);

            if (group.Status == "Approved")
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Group Approved. You cannot add topics." });
            }

            var reqTopic = new ReqTopicModel
            {
                GroupId = groupMember.GroupId,
                TopicCode = topicCode,
                TopicName = topicName,
                Description = $"Topic{groupCheck.GroupId}/{file.FileName}",
                Status = true
            };

            var obj = _mapper.Map<RegTopicForm>(reqTopic);
            var isTopicCreated = await _reqTopicService.CreateReqTopic(obj);

            var filePath = reqTopic.Description;

            using (var stream = file.OpenReadStream())
            {
                await _storageClient.UploadObjectAsync(_bucketName, filePath, file.ContentType, stream);
            }

            if (isTopicCreated)
            {
                var response = new ApiResponse<object>
                {
                    Data = new { Message = "Topic created successfully!" }
                };
                return Ok(response);
            }
            else
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Create fails." });
            }
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadReqTopic(int regTopicId)
        {
            var reqtopic = await _reqTopicService.GetReqTopicById(regTopicId);
            if (reqtopic == null)
                return NotFound("Document not found.");

            await _storageClient.GetObjectAsync(_bucketName, reqtopic.Description);

            var credential = GoogleCredential.FromFile(
                Path.Combine(Directory.GetCurrentDirectory(), "exeunitask-firebase-adminsdk-3jz7t-66373e3f35.json")
            ).UnderlyingCredential as ServiceAccountCredential;

            if (credential == null)
                return StatusCode(500, "Failed to load service account credentials.");

            var signedUrl = UrlSigner.FromCredential(credential).Sign(
                _bucketName,
                reqtopic.Description,
                TimeSpan.FromHours(1),
                HttpMethod.Get
            );

            return Ok(new { Url = signedUrl });
        }


        [Authorize(Roles = "Student")]
        [HttpPut]
        public async Task<IActionResult> UpdateReqTopic([FromBody] UpdateRegTopicModel reqTopic)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var role = await _groupMemberService.GetRoleByUserId(userId);

            if (role != "Leader")
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You are not a leader to perform this operation." });
            }

            var reqNew = await _reqTopicService.GetReqTopicById(reqTopic.RegTopicId);

            if (reqNew == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorMessage = "Không tìm thấy yêu cầu với ID đã cho." });
            }

            reqNew.Description = reqTopic.Description;
            reqNew.TopicName = reqTopic.TopicName;
            var isReqUpdated = await _reqTopicService.UpdateReqTopic(reqNew);

            ApiResponse<object> response = new ApiResponse<object>
            {
                Data = new
                {
                    Description = reqNew.Description,
                    TopicName = reqNew.TopicName
                }
            };

            if (isReqUpdated)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Không thể cập nhật Description.");
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPut("DeleteReq")]
        public async Task<IActionResult> DeleteReqTopic([FromBody] int RegTopicId)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var role = await _groupMemberService.GetRoleByUserId(userId);

            if (role != "Leader")
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You are not a leader to perform this operation." });
            }
            var reqNew = await _reqTopicService.GetReqTopicById(RegTopicId);
            reqNew.Status = false;
            var isReqUpdated = await _reqTopicService.UpdateReqTopic(reqNew);
            ApiResponse<object> response = new ApiResponse<object>
            {
                Data = new
                {
                    TopicName = reqNew.TopicName,
                    Description = reqNew.Description
                }
            };

            if (isReqUpdated)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Không thể cập nhật Description.");
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("MyTopic")]
        public async Task<IActionResult> GetMyReqTopics()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var groupMember = await _groupMemberService.GetGroupMemberByUserId(userId);
            if (groupMember == null || groupMember.GroupId == 0)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorMessage = "User does not belong to any group." });
            }

            var reqTopicList = await _reqTopicService.GetAllReqTopic();
            if (reqTopicList == null || !reqTopicList.Any())
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "No topics available." });
            }

            var responseData = reqTopicList
                .Where(reqTopic => reqTopic.Status == true && reqTopic.GroupId == groupMember.GroupId)
                .Select(reqTopic => new
                {
                    reqTopic.RegTopicId,
                    reqTopic.GroupId,
                    reqTopic.TopicCode,
                    reqTopic.TopicName,
                    reqTopic.Description,
                    reqTopic.Status,
                    GroupName = reqTopic.Group.GroupName,
                    SubjectCode = reqTopic.Group.Subject.SubjectCode
                });

            var response = new ApiResponse<IEnumerable<object>>
            {
                Success = true,
                Data = responseData
            };

            return Ok(response);
        }


    }
}
