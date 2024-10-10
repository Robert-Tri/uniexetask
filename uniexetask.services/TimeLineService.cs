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

        public async System.Threading.Tasks.Task UpdateTimeLine(Timeline timeLine)
        {
            var timeLineToUpdate = await _unitOfWork.TimeLines.GetByIDAsync(timeLine.TimelineId);
            if (timeLineToUpdate != null)
            {
                timeLineToUpdate.TimelineName = timeLine.TimelineName;
                timeLineToUpdate.Description = timeLine.Description;
                timeLineToUpdate.StartDate = timeLine.StartDate;
                timeLineToUpdate.EndDate = timeLine.EndDate;
                _unitOfWork.TimeLines.Update(timeLineToUpdate);
                _unitOfWork.Save();
            }
        }

        public void DeleteTimeLine(int timeLineId)
        {
            _unitOfWork.TimeLines.Delete(timeLineId);
            _unitOfWork.Save();
        }
    }
}
