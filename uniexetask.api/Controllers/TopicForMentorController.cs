using AutoMapper;
using Azure;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/topicMentor")]
    [ApiController]
    public class TopicForMentorController : ControllerBase
    {
        private readonly string _bucketName = "exeunitask.appspot.com";
        private readonly StorageClient _storageClient;
        private readonly IMapper _mapper;
        private readonly IProjectProgressService _projectProgressService;
        private readonly IProjectService _projectService;
        private readonly ITimeLineService _timeLineService;
        private readonly IMentorService _mentorService;
        private readonly ITopicForMentorService _topicMentorService;
        private readonly ITopicService _topicService;
        private readonly IReqTopicService _reqTopicService;
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly INotificationService _notificationService;

        public TopicForMentorController(
            StorageClient storageClient,
            IMapper mapper,
            IProjectProgressService projectProgressService,
            IProjectService projectService,
            ITimeLineService timeLineService,
            ITopicService topicService,
            IReqTopicService reqTopicService,
            IGroupService groupService,
            IGroupMemberService groupMemberService,
            IMentorService mentorService,
            ITopicForMentorService topicMentorService,
            INotificationService notificationService)
        {
            _projectProgressService = projectProgressService;
            _storageClient = storageClient;
            _mapper = mapper;
            _projectService = projectService;
            _timeLineService = timeLineService;
            _topicService = topicService;
            _reqTopicService = reqTopicService;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _notificationService = notificationService;
            _mentorService = mentorService;
            _topicMentorService = topicMentorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTopicMentorList()
        {
            var topicMentorList = await _topicMentorService.GetAllTopicForMentor();
            if (topicMentorList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<TopicForMentor>> response = new ApiResponse<IEnumerable<TopicForMentor>>();
            response.Data = topicMentorList;
            return Ok(response);
        }

        [Authorize(Roles = nameof(EnumRole.Mentor))]
        [HttpGet("MyTopicMentor")]
        public async Task<IActionResult> GetTopicMentorByUserId()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var mentor = await _mentorService.GetMentorWithGroupAsync(userId);

            var topicMentorList = await _topicMentorService.GetTopicForMentorByMentorId(mentor.MentorId);
            if (topicMentorList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<TopicForMentor>> response = new ApiResponse<IEnumerable<TopicForMentor>>();
            response.Data = topicMentorList;
            return Ok(response);
        }

        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpGet("ViewTopicMentor")]
        public async Task<IActionResult> ViewTopicMentor()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var group = await _groupService.GetGroupByUserId(userId);
            var mentor = await _mentorService.GetMentorByGroupId(group.GroupId);

            var topicMentorList = await _topicMentorService.GetTopicForMentorByMentorId(mentor.MentorId);
            if (topicMentorList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<TopicForMentor>> response = new ApiResponse<IEnumerable<TopicForMentor>>();
            response.Data = topicMentorList;
            return Ok(response);
        }

        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpPost("RegisteredTopic")]
        public async Task<IActionResult> RegisteredTopic([FromBody] TopicForMentorModel topicMentor)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var group = await _groupService.GetGroupByUserId(userId);

            var topic = new TopicModel
            {
                TopicCode = topicMentor.TopicCode,
                TopicName = topicMentor.TopicName,
                Description = topicMentor.Description
            };

            var objTopic = _mapper.Map<Topic>(topic);
            //var objTopicMentor = _mapper.Map<TopicForMentor>(topicMentor);

            var topicList = await _topicService.GetAllTopics();

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
                IsDeleted = false
            };

            var objProjectProgress = _mapper.Map<ProjectProgress>(projectProgress);
            var createProjectProgress = await _projectProgressService.CreateProjectProgress(objProjectProgress);

            var topicNew = await _topicMentorService.GetTopicForMentorByTopicCode(topicMentor.TopicCode);

            topicNew.IsRegistered = true;

            var isTopicMentorUpdated = await _topicMentorService.UpdateTopicMentor(topicNew);

            var groupUpdate = await _groupService.UpdateGroupApproved(group.GroupId);

            var reqTopicList = await _reqTopicService.GetReqTopicByGroupId(group.GroupId);

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

        [Authorize(Roles = nameof(EnumRole.Mentor))]
        [HttpPost]
        public async Task<IActionResult> CreateTopicMentor(IFormFile file, string topicName)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var mentor = await _mentorService.GetMentorWithGroupAsync(userId);
            var topicMentorCheck = await _topicMentorService.GetTopicForMentorByMentorId(mentor.MentorId);

            if (topicMentorCheck.Any(t => t.TopicName == topicName)) 
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "This topic name already exists" });
            }

            var existedTopics = await _topicMentorService.GetTopicMentorByDescription($"TopicMentor{mentor.MentorId}/{file.FileName}");
            if (existedTopics.Any())
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Description with the same name already exists." });

            var topicCodeMax = await _topicMentorService.GetMaxTopicCodeMentor();
            int nextCodeNumber = 1;
            if (!string.IsNullOrEmpty(topicCodeMax) && topicCodeMax.Length > 2)
            {
                var numericPart = topicCodeMax.Substring(2);
                if (int.TryParse(numericPart, out int currentMax))
                {
                    nextCodeNumber = currentMax + 1;
                }
            }
            var topicCode = $"TM{nextCodeNumber:D3}";

            var topicMentor = new TopicForMentorModel
            {
                MentorId = mentor.MentorId,
                TopicCode = topicCode,
                TopicName = topicName,
                Description = $"TopicMentor{mentor.MentorId}/{file.FileName}",
                IsRegistered = false
            };

            var obj = _mapper.Map<TopicForMentor>(topicMentor);
            var isTopicMentorCreated = await _topicMentorService.CreateTopicMentor(obj);
            var filePath = topicMentor.Description;

            using (var stream = file.OpenReadStream())
            {
                await _storageClient.UploadObjectAsync(_bucketName, filePath, file.ContentType, stream);
            }

            if (isTopicMentorCreated)
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

        [Authorize(Roles = nameof(EnumRole.Mentor))]
        [HttpPut]
        public async Task<IActionResult> UpdateTopicMentor(IFormFile file, string topicName, int TopicForMentorId)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var mentor = await _mentorService.GetMentorWithGroupAsync(userId);
            var topicMentorCheck = await _topicMentorService.GetTopicForMentorByMentorId(mentor.MentorId);

            if (topicMentorCheck.Any(t => t.TopicName == topicName))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "This topic name already exists" });
            }

            var topicNew = await _topicMentorService.GetTopicMentorById(TopicForMentorId);

            if (topicNew == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorMessage = "Không tìm thấy yêu cầu với ID đã cho." });
            }

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            topicNew.Description = $"TopicMentor{mentor.MentorId}/{file.FileName}";
            topicNew.TopicName = topicName;
            var isTopicMentorUpdated = await _topicMentorService.UpdateTopicMentor(topicNew);

            ApiResponse<object> response = new ApiResponse<object>
            {
                Data = new
                {
                    Description = topicNew.Description,
                    TopicName = topicNew.TopicName
                }
            };

            if (isTopicMentorUpdated)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Không thể cập nhật Description.");
            }
        }

        [Authorize]
        [HttpGet("download")]
        public async Task<IActionResult> DownloadTopicMentor(int topicForMentorId)
        {
            var topic = await _topicMentorService.GetTopicMentorById(topicForMentorId);
            if (topic == null)
                return NotFound("Document not found.");

            await _storageClient.GetObjectAsync(_bucketName, topic.Description);

            var credential = GoogleCredential.FromFile(
                Path.Combine(Directory.GetCurrentDirectory(), "exeunitask-firebase-adminsdk-3jz7t-66373e3f35.json")
            ).UnderlyingCredential as ServiceAccountCredential;

            if (credential == null)
                return StatusCode(500, "Failed to load service account credentials.");

            var signedUrl = UrlSigner.FromCredential(credential).Sign(
                _bucketName,
                topic.Description,
                TimeSpan.FromHours(1),
                HttpMethod.Get
            );

            return Ok(new { Url = signedUrl });
        }

        [Authorize(Roles = nameof(EnumRole.Mentor))]
        [HttpPut("DeleteTopicMentor")]
        public async Task<IActionResult> DeleteTopicMentor([FromBody] int TopicForMentorId)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var reqNew = await _topicMentorService.GetTopicMentorById(TopicForMentorId);
            reqNew.IsDeleted = true;
            var isReqUpdated = await _topicMentorService.UpdateTopicMentor(reqNew);
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
    }
}
