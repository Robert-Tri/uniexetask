using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IReqTopicService
    {
        Task<IEnumerable<RegTopicForm>> GetAllReqTopic();
        Task<bool> CreateReqTopic(RegTopicForm reqTopic);
        Task<string> GetMaxTopicCode();
    }
}
