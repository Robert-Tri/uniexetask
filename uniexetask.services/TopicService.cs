using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class TopicService : ITopicService
    {
        public IUnitOfWork _unitOfWork;
        public TopicService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Topic>> GetAllTopics()

        {
            var topic = await _unitOfWork.Topics.GetAllTopics();
            return topic;
        }
    }
}
