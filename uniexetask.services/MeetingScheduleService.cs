using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
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

        public async Task<MeetingSchedule?> CreateMeetingSchedule(MeetingSchedule meetingSchedule)
        {
            var mentorExists = await _unitOfWork.Mentors.GetByIDAsync(meetingSchedule.MentorId);
            var groupExists = await _unitOfWork.Groups.GetByIDAsync(meetingSchedule.GroupId);

            if (mentorExists == null || groupExists == null)
            {
                throw new Exception("Mentor or group do not exist.");
            }
            var mentor = await _unitOfWork.Groups.GetMentorInGroup(groupExists.GroupId);
            if (mentor == null) throw new Exception($"There are no mentors in {groupExists.GroupName} group.");
            if (mentor.UserId != mentorExists.UserId) throw new Exception($"You are not a mentor of the {groupExists.GroupName} group.");
            await _unitOfWork.MeetingSchedules.InsertAsync(meetingSchedule);
            _unitOfWork.Save();
            
            return meetingSchedule;
        }

        public async Task<bool> DeleteMeetingSchedule(int meetingScheduleId)
        {
            var meetingSchedule = await _unitOfWork.MeetingSchedules.GetByIDAsync(meetingScheduleId);
            if (meetingSchedule == null) throw new Exception("Meeting not found.");
            //_unitOfWork.MeetingSchedules.Delete(meetingSchedule);
            meetingSchedule.IsDeleted = true;
            _unitOfWork.MeetingSchedules.Update(meetingSchedule);
            _unitOfWork.Save();
            return true;
        }

        public async Task<MeetingSchedule?> EditMeetingSchedule(int meetingScheduleId, MeetingSchedule meeting)
        {
            if (meeting.Type == nameof(MeetingScheduleType.Offline)) meeting.Url = null;
            var meetingSchedule = await _unitOfWork.MeetingSchedules.GetByIDAsync(meetingScheduleId);
            if (meetingSchedule == null) throw new Exception("Meeting schedule not found.");
            meetingSchedule.Content = meeting.Content;
            meetingSchedule.Duration = meeting.Duration;
            meetingSchedule.Location = meeting.Location;
            meetingSchedule.Type = meeting.Type;
            meetingSchedule.Url = meeting.Url;
            meetingSchedule.MeetingScheduleName = meeting.MeetingScheduleName;
            meetingSchedule.MeetingDate = meeting.MeetingDate;
            _unitOfWork.MeetingSchedules.Update(meetingSchedule);
            _unitOfWork.Save();
            return meetingSchedule;
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
