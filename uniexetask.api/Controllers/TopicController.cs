using AutoMapper;
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

        private readonly ITopicService _topicService;
        private readonly IMapper _mapper;
        public TopicController(ITopicService userService, IMapper mapper)
        {
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
    }
}