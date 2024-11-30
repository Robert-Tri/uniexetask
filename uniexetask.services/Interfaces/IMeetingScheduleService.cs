using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IMeetingScheduleService
    {
        Task<MeetingSchedule?> CreateMeetingSchedule(MeetingSchedule meetingSchedule);
        Task<bool> DeleteMeetingSchedule(int meetingScheduleId);
        Task<MeetingSchedule?> EditMeetingSchedule(int meetingScheduleId, MeetingSchedule meetingSchedule);
        Task<MeetingSchedule?> GetMeetingScheduleByMeetingScheduleId(int meetingScheduleId);
        Task<IEnumerable<MeetingSchedule>> GetMeetingSchedulesByGroupId(int groupId);
    }
}
