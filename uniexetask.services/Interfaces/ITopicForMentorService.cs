using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface ITopicForMentorService
    {
        Task<IEnumerable<TopicForMentor>> GetAllTopicForMentor();
        Task<bool> CreateTopicMentor(TopicForMentor topicMentor);
        Task<string> GetMaxTopicCodeMentor();
        Task<TopicForMentor> GetTopicMentorById(int id);
        Task<IEnumerable<TopicForMentor>> GetTopicForMentorByMentorId(int mentorId);
        Task<bool> UpdateTopicMentor(TopicForMentor Topics);
        Task<TopicForMentor> GetTopicForMentorByTopicCode(string topicCode);
        Task<List<TopicForMentor>> GetTopicMentorByDescription(string description);
    }
}
