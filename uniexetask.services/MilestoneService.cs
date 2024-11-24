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
    public class MilestoneService : IMilestoneService
    {
        public IUnitOfWork _unitOfWork;

        public MilestoneService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Milestone?> GetMilestoneWithCriteria(int id)
        {
            return await _unitOfWork.Milestones.GetMileStoneWithCriteria(id);
        }

        public async Task<IEnumerable<Milestone>> GetMileStones()
        {
            return await _unitOfWork.Milestones.GetAsync();
        }

        public async Task<IEnumerable<Milestone>> GetMileStonesBySubjectId(int subjectId)
        {
            return await _unitOfWork.Milestones.GetAsync(filter: m => m.SubjectId == subjectId);
        }
    }
}
