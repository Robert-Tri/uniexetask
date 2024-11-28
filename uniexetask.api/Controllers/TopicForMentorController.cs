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
        private readonly IMentorService _mentorService;
        private readonly ITopicForMentorService _topicMentorService;

        public TopicForMentorController(
            StorageClient storageClient,
            IMapper mapper,
            IMentorService mentorService,
            ITopicForMentorService topicMentorService)
        {
            _storageClient = storageClient;
            _mapper = mapper;
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
    }
}
