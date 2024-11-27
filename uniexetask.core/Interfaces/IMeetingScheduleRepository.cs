using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IMeetingScheduleRepository : IGenericRepository<MeetingSchedule>
    {
        Task<IEnumerable<MeetingSchedule>> GetMeetingSchedulesByGroupId(int groupId);
    }
}
