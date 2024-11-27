using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class MeetingScheduleService : IMeetingScheduleService
    {
        public IUnitOfWork _unitOfWork;

        public MeetingScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MeetingSchedule?> GetMeetingScheduleByMeetingScheduleId(int meetingScheduleId)
        {
            return await _unitOfWork.MeetingSchedules.GetByIDAsync(meetingScheduleId);
        }

        public async Task<IEnumerable<MeetingSchedule>> GetMeetingSchedulesByGroupId(int groupId)
        {
            return await _unitOfWork.MeetingSchedules.GetMeetingSchedulesByGroupId(groupId);
        }
    }
}
