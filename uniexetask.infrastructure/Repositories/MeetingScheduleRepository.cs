using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;

namespace uniexetask.infrastructure.Repositories
{
    public class MeetingScheduleRepository : GenericRepository<MeetingSchedule>, IMeetingScheduleRepository
    {
        public MeetingScheduleRepository(UniExetaskContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<MeetingSchedule>> GetMeetingSchedulesByGroupId(int groupId)
        {
            return await dbSet.Where(m => m.GroupId == groupId).ToListAsync();
        }
    }
}
