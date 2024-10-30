using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface ITimeLineService
    {
        Task<IEnumerable<Timeline>> GetTimeLines();

        System.Threading.Tasks.Task CreateTimeLine(Timeline timeLine);

        System.Threading.Tasks.Task UpdateTimeLine(DateTime startDate, int subjectId);

        void DeleteTimeLine(int timeLineId);
    }
}
