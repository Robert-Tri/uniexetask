using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;

namespace uniexetask.infrastructure.Repositories
{
    class TopicForMentorRepsitory : GenericRepository<TopicForMentor>, ITopicForMentorRepository
    {
        public TopicForMentorRepsitory(UniExetaskContext dbContext) : base(dbContext)
        {
        }
    }
}
