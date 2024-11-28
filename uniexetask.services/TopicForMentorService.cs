using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class TopicForMentorService : ITopicForMentorService
    {
        public IUnitOfWork _unitOfWork;
        public TopicForMentorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<TopicForMentor>> GetAllTopicForMentor()
        {
            var topicMentorList = await _unitOfWork.TopicForMentor.GetAsync();
            return topicMentorList;
        }

        public async Task<IEnumerable<TopicForMentor>> GetTopicForMentorByMentorId(int mentorId)
        {
            var topicMentorList = await _unitOfWork.TopicForMentor.GetAsync(
                filter: t => t.MentorId == mentorId
            );
            return topicMentorList;
        }


        public async Task<TopicForMentor> GetTopicMentorById(int id)
        {
            var mentorTopic = await _unitOfWork.TopicForMentor.GetByIDAsync(id);
            return mentorTopic;
        }

        public async Task<bool> CreateTopicMentor(TopicForMentor topicMentor)
        {
            if (topicMentor != null)
            {
                await _unitOfWork.TopicForMentor.InsertAsync(topicMentor);

                var result = _unitOfWork.Save();

                if (result > 0) 
                    return true;
                else
                    return false;
            }
            return false;
        }

        public async Task<string> GetMaxTopicCodeMentor()
        {
            var reqTopicList = await _unitOfWork.TopicForMentor.GetAsync();
            var maxTopicCode = reqTopicList
                .Where(t => t.TopicCode.StartsWith("TM"))
                .OrderByDescending(t => t.TopicCode)
                .Select(t => t.TopicCode)
                .FirstOrDefault();

            return maxTopicCode;
        }
    }
}
