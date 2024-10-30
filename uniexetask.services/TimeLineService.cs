using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Enums;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
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

        public async System.Threading.Tasks.Task CreateTimeLine(Timeline timeLine)
        {
            await _unitOfWork.TimeLines.InsertAsync(timeLine);
            _unitOfWork.Save();
        }

        public async System.Threading.Tasks.Task UpdateTimeLine(DateTime startDate, int subjectId)
        {
            var timeLines = await _unitOfWork.TimeLines.GetAsync(filter: t => t.SubjectId == subjectId);
            foreach (var timeLine in timeLines) 
            {
                switch (timeLine.TimelineId)
                {
                    case (int)TimelineType.AssignMentor:
                        timeLine.EndDate = startDate.AddDays(7);
                        break;
                    case (int)TimelineType.SelectTopic:
                        timeLine.EndDate = startDate.AddDays(14);
                        break;
                    default:
                        break;
                }
                _unitOfWork.TimeLines.Update(timeLine);
            }
            _unitOfWork.Save();
        }

        public void DeleteTimeLine(int timeLineId)
        {
            _unitOfWork.TimeLines.Delete(timeLineId);
            _unitOfWork.Save();
        }
    }
}
