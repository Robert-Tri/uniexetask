using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/topic")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly string _bucketName = "exeunitask.appspot.com";
        private readonly StorageClient _storageClient;
        private readonly ITopicService _topicService;
        private readonly IMapper _mapper;
        public TopicController(StorageClient storageClient, ITopicService userService, IMapper mapper)
        {
            _storageClient = storageClient;
            _topicService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetTopicList()
        {
            var topicList = await _topicService.GetAllTopics();
            if (topicList == null)
            {
                return NotFound();
            }
            List<TopicListModel> topics = new List<TopicListModel>();
            foreach (var topic in topicList)
            {
                if (topic != null) topics.Add(new TopicListModel
                {
                    TopicId = topic.TopicId,
                    TopicCode = topic.TopicCode,
                    TopicName = topic.TopicName,
                    Description = topic.Description
                });
            }
            ApiResponse<IEnumerable<TopicListModel>> response = new ApiResponse<IEnumerable<TopicListModel>>();
            response.Data = topics;
            return Ok(response);
        }

        [Authorize]
        [HttpGet("download")]
        public async Task<IActionResult> DownloadTopic(int TopicId)
        {
            var topic = await _topicService.GetTopicById(TopicId);
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