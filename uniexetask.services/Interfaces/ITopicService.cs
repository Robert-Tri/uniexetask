using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface ITopicService
    {
        Task<IEnumerable<Topic>> GetAllTopics();
        Task<int> CreateTopic(Topic topic);
    }
}
