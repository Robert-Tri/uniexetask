using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class TimeLineService : ITimeLineService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TimeLineService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Timeline>> GetTimeLines()
        {
            return await _unitOfWork.TimeLines.GetAsync();
        }

        /*public async System.Threading.Tasks.Task CreateTimeLine(Timeline timeLine)
        {
            await _unitOfWork.TimeLines.InsertAsync(timeLine);
            _unitOfWork.Save();
        }*/

        public async System.Threading.Tasks.Task UpdateMainTimeLine(DateTime startDate, int subjectId)
        {
            var timeLines = await _unitOfWork.TimeLines.GetAsync(filter: t => t.SubjectId == subjectId);
            foreach (var timeLine in timeLines)
            {
                switch (timeLine.TimelineId)
                {
                    case (int)TimelineType.FinalizeGroupEXE101:
                        timeLine.EndDate = startDate.AddDays(7);
                        break;
                    case (int)TimelineType.FinalizeGroupEXE201:
                        timeLine.EndDate = startDate.AddDays(7);
                        break;
                    case (int)TimelineType.FinalizeMentorEXE101:
                        timeLine.EndDate = startDate.AddDays(14);
                        break;
                    case (int)TimelineType.FinalizeMentorEXE201:
                        timeLine.EndDate = startDate.AddDays(14);
                        break;
                    default:
                        break;
                }
                _unitOfWork.TimeLines.Update(timeLine);
            }
            _unitOfWork.Save();
        }
        public async Task<bool> UpdateSpecificTimeLine(int timeLineId, DateTime startDate, DateTime endDate, int subjectId)
        {
            var specificTimeline = await _unitOfWork.TimeLines.GetByIDAsync(timeLineId);
            if (specificTimeline == null)
                return false;

            var timelines = (await _unitOfWork.TimeLines.GetAsync(filter: t => t.TimelineId < timeLineId && t.SubjectId == subjectId)).ToList();

            foreach (var timeline in timelines)
            {
                if (startDate < timeline.StartDate || endDate < timeline.EndDate)
                    return false;
            }

            specificTimeline.StartDate = startDate;
            specificTimeline.EndDate = endDate;
            _unitOfWork.TimeLines.Update(specificTimeline);

            return _unitOfWork.Save() > 0;
        }


        public void DeleteTimeLine(int timeLineId)
        {
            _unitOfWork.TimeLines.Delete(timeLineId);
            _unitOfWork.Save();
        }
    }
}
