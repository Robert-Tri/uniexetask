using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;

namespace uniexetask.services.Interfaces
{
    public interface IMeetingScheduleService
    {
        Task<MeetingScheduleResponse> CreateMeetingSchedule(int userId, MeetingScheduleModel model);
        Task<bool> DeleteMeetingSchedule(int userId, int meetingScheduleId);
        Task<bool> EditMeetingSchedule(int userId, int meetingScheduleId, MeetingScheduleModel model);
        Task<MeetingSchedule?> GetMeetingScheduleByMeetingScheduleId(int meetingScheduleId);
        Task<IEnumerable<MeetingScheduleResponse>> GetMeetingSchedules(int userId, string role);
        Task<IEnumerable<MeetingSchedule>> GetMeetingSchedulesByGroupId(int groupId);
    }
}
