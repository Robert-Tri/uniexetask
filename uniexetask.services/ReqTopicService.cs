using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class ReqTopicService : IReqTopicService
    {
        public IUnitOfWork _unitOfWork;
        public ReqTopicService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<RegTopicForm>> GetAllReqTopic()
        {
            var reqTopicList = await _unitOfWork.ReqTopic.GetAsync();
            return reqTopicList;
        }

        public async Task<bool> CreateReqTopic(RegTopicForm reqTopic)
        {
            if (reqTopic != null)
            {
                await _unitOfWork.ReqTopic.InsertAsync(reqTopic);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public async Task<string> GetMaxTopicCode()
        {
            var reqTopicList = await _unitOfWork.ReqTopic.GetAsync();
            var maxTopicCode = reqTopicList
                .Where(t => t.TopicCode.StartsWith("TP"))
                .OrderByDescending(t => t.TopicCode)
                .Select(t => t.TopicCode)
                .FirstOrDefault();

            return maxTopicCode;
        }
    }
}
